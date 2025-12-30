/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System.Collections.Generic;
using Unity.Properties;

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
}
