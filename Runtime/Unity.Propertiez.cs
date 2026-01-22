/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using Unity.Properties;

namespace Unity.Properties
{
  public abstract class ContainerPropertyBagEx<TContainer> : ContainerPropertyBag<TContainer>
  {
    public new void AddProperty<TValue>(Property<TContainer, TValue> property) => base.AddProperty(property);
  }
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

    if (TypeConversion.TryConvert(ref Value, out TValue v))
      property.SetValue(ref container, v);
    else
      ReturnCode = VisitReturnCode.InvalidCast;
  }
}

static class Propertiez
{
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
