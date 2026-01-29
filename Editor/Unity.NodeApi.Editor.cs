using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Assertions;

#nullable enable

class UnityNodeApiBuild : IPreprocessBuildWithContext, IPostprocessBuildWithContext
{
  string? m_il2cppArgs;
  string? m_emsdkArgs;

  public int callbackOrder { get => 0; }

  public void OnPreprocessBuild(BuildCallbackContext ctx)
  {
    m_il2cppArgs = PlayerSettings.GetAdditionalIl2CppArgs();
    m_emsdkArgs = PlayerSettings.WebGL.emscriptenArgs;

    switch (ctx.Report.summary.platform)
    {
      case BuildTarget.StandaloneWindows64:
        PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs + " --linker-flags=\"/LIBPATH:Packages/net.ringoz.unity.nodeapi/Runtime/lib/ fcontext.lib napi.lib\"");
        break;
      case BuildTarget.StandaloneOSX:
        PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs + " --linker-flags=\"-LPackages/net.ringoz.unity.nodeapi/Runtime/lib -lfcontext -undefined dynamic_lookup\"");
        break;
      case BuildTarget.WebGL:
        PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs + " -sEXPORTED_FUNCTIONS=['_main','_malloc','_free','_napi_register_wasm_v1','_node_api_module_get_api_version_v1'] -sEXTRA_EXPORTED_RUNTIME_METHODS=['emnapiInit']";
        break;
    }
  }

  public void OnPostprocessBuild(BuildCallbackContext ctx)
  {
    PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs);
    PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs;

    // force class loads
    Node.Create("");

    var assembly = typeof(Node).Assembly;
    var attribs = assembly.GetCustomAttributes(typeof(GeneratePropertyBagsForTypeAttribute));
    var coreTypes = attribs.Select((attrib) => ((GeneratePropertyBagsForTypeAttribute)attrib).Type).ToArray();
    CoreTypes = new HashSet<Type>(coreTypes);

    var types = PropertyBag.GetAllTypesWithAPropertyBag().Where(type => !CoreTypes.Contains(type));
    var userTypes = types.Where(type => CoreTypes.Any(core => core.IsAssignableFrom(type))).ToArray();
    UserTypes = new HashSet<Type>(userTypes);

    string projectPath = Path.GetDirectoryName(Application.dataPath);
    GenerateTypings(coreTypes, projectPath + "/Packages/net.ringoz.unity.nodeapi/react.ts");
    GenerateTypings(userTypes, projectPath + "/index.ts");
  }

  static ISet<Type> CoreTypes = null!;
  static ISet<Type> UserTypes = null!;

  class PropNameComparer : EqualityComparer<IProperty>
  {
    public static EqualityComparer<IProperty> Instance = new PropNameComparer();

    public override bool Equals(IProperty x, IProperty y)
    {
      return x.Name.Equals(y.Name);
    }

    public override int GetHashCode(IProperty obj)
    {
      return obj.Name.GetHashCode();
    }
  }

  static IEnumerable<IProperty> GetProperties(Type type)
  {
    if (type == null)
      return Enumerable.Empty<IProperty>();
    var bag = PropertyBag.GetPropertyBag(type);
    if (bag == null)
      return GetProperties(type.BaseType);
    var props = (IEnumerable<IProperty>)bag.GetType().GetMethod("GetProperties", Array.Empty<Type>()).Invoke(bag, null);
    return props.Union(GetProperties(type.BaseType), PropNameComparer.Instance);
  }

  static IEnumerable<IProperty> GetOwnProperties(Type type)
  {
    return GetProperties(type).Except(GetProperties(type.BaseType), PropNameComparer.Instance);
  }

  static Type? TypeBase(Type type)
  {
    Type result;
    for (result = type.BaseType; result != null; result = result.BaseType)
      if (PropertyBag.GetPropertyBag(result) != null)
        break;
    return result;
  }

  static string TypeName(Type type, bool allowGenericArgs = true)
  {
    if (type == typeof(UnityEngine.Object))
      return "ObjectBase";

    if (!type.IsGenericType)
      return type.Name;

    // Extract the name without the backtick and arity (e.g., "List`1" -> "List")
    string genericName = type.Name.Substring(0, type.Name.IndexOf('`'));
    if (!allowGenericArgs) return genericName;

    // Recursively format each generic argument
    var genericArguments = type.GetGenericArguments().Select(t => TypeName(t));
    return $"{genericName}<{string.Join(", ", genericArguments)}>";
  }

  static string PropTypeName(Type type)
  {
    string name = TypeName(type);
    return IsPtrType(type) ? $"Ptr<{name}>" : name;
  }

  static MethodInfo IsPropTypeSupportedMethod = typeof(Node).GetMethod(nameof(Node.IsPropTypeSupported), Array.Empty<Type>());
  static ISet<Type> cache = new HashSet<Type>();

  static bool IsPropTypeSupported(Type type)
  {
    if (IsPtrType(type) || cache.Contains(type)) return true;
    return (bool)IsPropTypeSupportedMethod.MakeGenericMethod(type).Invoke(null, null);
  }

  static bool IsPtrType(Type type)
  {
    return CoreTypes.Contains(type) || UserTypes.Contains(type);
  }

  static IEnumerable<string> EmitType(Type type)
  {
    if (!cache.Add(type))
      yield break;

    using (var writer = new StringWriter())
    {
      Type? typeBase = TypeBase(type);
      if (typeBase != null)
        writer.WriteLine($"export interface {TypeName(type)} extends {TypeName(typeBase)} {{");
      else
        writer.WriteLine($"export interface {TypeName(type)} {{");

      foreach (var property in GetOwnProperties(type))
      {
        bool isArray = false;
        Type propType = property.DeclaredValueType();
        if (propType.IsEnum)
        {
          isArray = propType.GetCustomAttribute<FlagsAttribute>() != null;
          if (cache.Add(propType))
            yield return $"export type {TypeName(propType)} = {string.Join(" | ", Enum.GetNames(propType).Select(name => $"'{name}'"))};";
        }

        writer.Write(IsPropTypeSupported(propType) ? "  " : "  //");
        writer.WriteLine($"{(property.IsReadOnly ? "readonly " : "")}{property.Name}: {PropTypeName(propType)}{(isArray ? "[]" : "")};");
      }

      writer.WriteLine($"}}");
      if (typeof(UnityEngine.Object) != type && (typeof(UnityEngine.Object).IsAssignableFrom(type) || type.GetConstructor(Type.EmptyTypes) != null))
        writer.WriteLine($"export const {TypeName(type, false)} = intrinsic<{TypeName(type)}>(\"{type.Name}\");");

      yield return writer.ToString();
    }
  }

  static void GenerateTypings(IEnumerable<Type> types, string path)
  {
    Assert.IsTrue(File.Exists(path));

    var source = new StringBuilder(File.ReadAllText(path).Split("//#region generated")[0]);
    using (var writer = new StringWriter(source))
    {
      writer.WriteLine("//#region generated");
      writer.WriteLine();
      
      var texts = types.SelectMany(type => EmitType(type));
      foreach (string text in texts)
        writer.WriteLine(text);

      writer.WriteLine("//#endregion generated");
      File.WriteAllText(path, source.ToString());
    }
  }
}
