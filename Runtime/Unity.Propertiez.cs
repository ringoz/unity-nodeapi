/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System.Diagnostics;
using Unity.Properties;

class PropertiezDump : PropertyVisitor
{
  private delegate void WriteLine(string line);
  private readonly WriteLine mWriteLine = (line) => Trace.WriteLine(line);
  private int mIndentLevel = 0;

  protected override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
  {
    var propertyName = property switch
    {
      ICollectionElementProperty => $"[{property.Name}]",
      _ => property.Name
    };
    var type = value?.GetType() ?? property.DeclaredValueType();
    var typeName = TypeUtility.GetTypeDisplayName(type);

    string indent = new(' ', mIndentLevel * 2);
    if (TypeTraits.IsContainer(type))
      mWriteLine($"{indent}- {propertyName} {{{typeName}}}");
    else
      mWriteLine($"{indent}- {propertyName} = {{{typeName}}} {value}");

    ++mIndentLevel;
    if (null != value && (object)value != (object)container)
      PropertyContainer.Accept(this, ref value);
    --mIndentLevel;
  }
}
