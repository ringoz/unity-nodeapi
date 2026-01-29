/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JavaScript.NodeApi;
using Unity.Properties;

namespace Unity.Properties
{
  public abstract class ContainerPropertyBagEx<TContainer> : ContainerPropertyBag<TContainer>
  {
    public new void AddProperty<TValue>(Property<TContainer, TValue> property) => base.AddProperty(property);
  }
}

static class Propertiez
{
  public static bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
  {
    return TypeConversion.TryConvert(ref source, out destination);
  }

  class GetValueVisitor<TSrcValue> : PathVisitor
  {
    public static readonly UnityEngine.Pool.ObjectPool<GetValueVisitor<TSrcValue>> Pool = new UnityEngine.Pool.ObjectPool<GetValueVisitor<TSrcValue>>(() => new GetValueVisitor<TSrcValue>(), null, v => v.Reset());
    public TSrcValue Value;

    protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
    {
      if (!TryConvert(ref value, out Value))
        ReturnCode = VisitReturnCode.InvalidCast;
    }
  }

  public static bool TryGetValue<TValue>(object container, in PropertyPath path, out TValue value, out VisitReturnCode returnCode)
  {
    var visitor = GetValueVisitor<TValue>.Pool.Get();
    visitor.Path = path;
    visitor.ReadonlyVisit = true;
    try
    {
      returnCode = VisitReturnCode.MissingPropertyBag;
      for (var type = container.GetType(); type != null; type = type.BaseType)
      {
        var properties = PropertyBag.GetPropertyBag(type);
        if (null != properties)
        {
          properties.Accept(visitor, ref container);
          if ((returnCode = visitor.ReturnCode) == VisitReturnCode.Ok)
            break;

          visitor.Reset();
          visitor.Path = path;
          visitor.ReadonlyVisit = true;
        }
      }
      value = visitor.Value;
    }
    finally
    {
      visitor.Value = default;
      GetValueVisitor<TValue>.Pool.Release(visitor);
    }

    return returnCode == VisitReturnCode.Ok;
  }

  public static TValue GetValue<TValue>(object container, in PropertyPath path)
  {
    if (path.IsEmpty)
      throw new InvalidPathException("The specified PropertyPath is empty.");

    if (TryGetValue(container, path, out TValue value, out var returnCode))
      return value;

    switch (returnCode)
    {
      case VisitReturnCode.NullContainer:
        throw new ArgumentNullException(nameof(container));
      case VisitReturnCode.InvalidContainerType:
        throw new InvalidContainerTypeException(container.GetType());
      case VisitReturnCode.MissingPropertyBag:
        throw new MissingPropertyBagException(container.GetType());
      case VisitReturnCode.InvalidCast:
        throw new InvalidCastException($"Failed to GetValue of Type=[{typeof(TValue).Name}] for property with path=[{path}]");
      case VisitReturnCode.InvalidPath:
        throw new InvalidPathException($"Failed to GetValue for property with Path=[{path}]");
      default:
        throw new Exception($"Unexpected {nameof(VisitReturnCode)}=[{returnCode}]");
    }
  }

  public static JSValue GetJSValue(object container, in PropertyPath path)
  {
    return GetValue<JSValue>(container, path);
  }

  class SetValueVisitor<TSrcValue> : PathVisitor
  {
    public static readonly UnityEngine.Pool.ObjectPool<SetValueVisitor<TSrcValue>> Pool = new(() => new SetValueVisitor<TSrcValue>(), null, v => v.Reset());
    public TSrcValue Value;

    protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
    {
      if (property.IsReadOnly)
      {
        ReturnCode = VisitReturnCode.AccessViolation;
        return;
      }

      if (TryConvert(ref Value, out TValue v))
        property.SetValue(ref container, v);
      else
        ReturnCode = VisitReturnCode.InvalidCast;
    }
  }

  public static bool TrySetValue<TValue>(object container, in PropertyPath path, in TValue value, out VisitReturnCode returnCode)
  {
    var visitor = SetValueVisitor<TValue>.Pool.Get();
    visitor.Path = path;
    visitor.Value = value;
    try
    {
      returnCode = VisitReturnCode.MissingPropertyBag;
      for (var type = container.GetType(); type != null; type = type.BaseType)
      {
        var properties = PropertyBag.GetPropertyBag(type);
        if (null != properties)
        {
          properties.Accept(visitor, ref container);
          if ((returnCode = visitor.ReturnCode) == VisitReturnCode.Ok)
            break;

          visitor.Reset();
          visitor.Path = path;
        }
      }
    }
    finally
    {
      visitor.Value = default;
      SetValueVisitor<TValue>.Pool.Release(visitor);
    }

    return returnCode == VisitReturnCode.Ok;
  }

  public static void SetValue<TValue>(object container, in PropertyPath path, TValue value)
  {
    if (TrySetValue(container, path, value, out var returnCode))
      return;

    switch (returnCode)
    {
      case VisitReturnCode.NullContainer:
        throw new ArgumentNullException(nameof(container));
      case VisitReturnCode.InvalidContainerType:
        throw new InvalidContainerTypeException(container.GetType());
      case VisitReturnCode.MissingPropertyBag:
        throw new MissingPropertyBagException(container.GetType());
      case VisitReturnCode.InvalidCast:
        throw new InvalidCastException($"Failed to SetValue of Type=[{typeof(TValue).Name}] for property with path=[{path}]");
      case VisitReturnCode.InvalidPath:
        throw new InvalidPathException($"Failed to SetValue for property with Path=[{path}]");
      case VisitReturnCode.AccessViolation:
        throw new AccessViolationException($"Failed to SetValue for read-only property with Path=[{path}]");
      default:
        throw new Exception($"Unexpected {nameof(VisitReturnCode)}=[{returnCode}]");
    }
  }

  public static void SetJSValue(object container, in PropertyPath path, in JSValue value)
  {
    switch (value.TypeOf())
    {
      case JSValueType.String:
        if (!TrySetValue(container, path, (string)value, out _))
          SetValue(container, path, value);
        break;

      case JSValueType.External:
        var obj = value.TryGetValueExternal();
        if (obj is UnityEngine.Object ueo)
          SetValue(container, path, ueo);
        else
          SetValue(container, path, obj);
        break;

      default:
        var part = path[path.Length - 1];
        if (part.IsName && (part.Name.EndsWith("Flags") || part.Name.EndsWith("Hints")) && value.IsArray())
          SetValue(container, path, string.Join(',', ((JSArray)value).Select(v => (string)v)));
        else
          SetValue(container, path, value);
        break;
    }
  }
}

class PropertiezDump : PropertyVisitor
{
  public delegate void WriteLine(string line);
  private readonly WriteLine mWriteLine;
  private readonly HashSet<object> mIndent = new();

  public PropertiezDump(WriteLine writeLine)
  {
    mWriteLine = writeLine;
  }

  protected override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
  {
    var propertyName = property switch
    {
      ICollectionElementProperty => $"[{property.Name}]",
      _ => property.Name
    };
    var type = value?.GetType() ?? property.DeclaredValueType();
    var typeName = TypeUtility.GetTypeDisplayName(type);

    string indent = new(' ', mIndent.Count * 2);
    if (TypeTraits.IsContainer(type))
      mWriteLine($"{indent}- {propertyName} {{{typeName}}}");
    else
      mWriteLine($"{indent}- {propertyName} = {{{typeName}}} {value}");

    mIndent.Add(container);
    if (null != value && !mIndent.Contains(value))
      PropertyContainer.Accept(this, ref value);
    mIndent.Remove(container);
  }

  public static string ToString(object container)
  {
    var sb = new StringBuilder();
    PropertyContainer.Accept(new PropertiezDump((l) => sb.AppendLine(l)), container);
    return sb.ToString();
  }
}
