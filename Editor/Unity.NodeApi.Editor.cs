using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Assertions;

class UnityNodeApiBuild : IPreprocessBuildWithContext, IPostprocessBuildWithContext
{
  string m_il2cppArgs;
  string m_emsdkArgs;

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
        PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs + " -sEXPORTED_FUNCTIONS=['_malloc','_free','_napi_register_wasm_v1','_node_api_module_get_api_version_v1'] -sEXTRA_EXPORTED_RUNTIME_METHODS=['emnapiInit']";
        break;
    }
  }

  public void OnPostprocessBuild(BuildCallbackContext ctx)
  {
    PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs);
    PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs;

    string projectPath = Path.GetDirectoryName(Application.dataPath);
    GenerateTypings(EnumCoreTypes(), projectPath + "/Packages/net.ringoz.unity.nodeapi/react.ts");
    GenerateTypings(EnumUserTypes(), projectPath + "/index.ts");
  }

  static IEnumerable<Type> EnumCoreTypes()
  {
    yield return typeof(GameObject);
    yield return typeof(Component);
  }

  static IEnumerable<Type> EnumUserTypes()
  {
    var types = PropertyBag.GetAllTypesWithAPropertyBag();
    return types.Where(type => EnumCoreTypes().Any(core => type.IsSubclassOf(core)));
  }

  static IEnumerable<IProperty> GetProperties(Type type)
  {
    var bag = PropertyBag.GetPropertyBag(type);
    if (bag == null)
      return Enumerable.Empty<IProperty>();
    return (IEnumerable<IProperty>)bag.GetType().GetMethod("GetProperties", Array.Empty<Type>()).Invoke(bag, null);
  }

  static IEnumerable<IProperty> GetOwnProperties(Type type)
  {
    var basePropNames = GetProperties(type.BaseType).Select(p => p.Name);
    return GetProperties(type).Where(p => !basePropNames.Contains(p.Name));
  }

  static void EmitType(Type type, TextWriter writer)
  {
    writer.WriteLine($"export interface {type.Name} extends {type.BaseType.Name} {{");
    foreach (var property in GetOwnProperties(type))
      writer.WriteLine($"  {(property.IsReadOnly ? "readonly " : "")}{property.Name}: {property.DeclaredValueType().Name};");

    writer.WriteLine($"}}");
    writer.WriteLine($"export const {type.Name} = intrinsic<{type.Name}>(\"{type.Name}\");");
  }

  static void GenerateTypings(IEnumerable<Type> types, string path)
  {
    Assert.IsTrue(File.Exists(path));
    string preamble = File.ReadAllText(path).Split("//#region generated")[0];

    using (StreamWriter writer = new StreamWriter(path))
    {
      writer.Write(preamble);
      writer.WriteLine("//#region generated");
      writer.WriteLine();
      foreach (Type type in types)
      {
        EmitType(type, writer);
        writer.WriteLine();
      }
      writer.WriteLine("//#endregion generated");
    }
  }
}
