/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using Microsoft.JavaScript.NodeApi;
using UnityEngine.Assertions;

#nullable enable

class AttributeOverridesNode : Node
{
  protected object mName = null!;

  public override object Ptr => mPtr is JSReference ? null! : mPtr;

  protected AttributeOverridesNode(object obj) : base(obj) { }

  public static new Node? Create(object kind) => kind switch
  {
    string name => name.StartsWith("#") ? new AttributeOverridesNode(null!) { mName = name.Substring(1) } : null,
    _ => null
  };

  public override void Dispose() => (mPtr as JSReference)?.Dispose();

  public override string ToString() => $"[#{mName}] {mPtr}";

  public override void SetProps(in JSValue props)
  {
    if (mPtr is JSReference reference)
    {
      reference.Dispose();
      mPtr = null!;
    }

    if (mPtr == null)
      mPtr = new JSReference(props);
    else
      base.SetProps(props);
  }

  public override void SetParent(Node? parent, Node? beforeChild)
  {
    if (parent == null)
    {
      mPtr = null!;
      return;
    }

    using (var reference = (JSReference)mPtr)
    {
      mPtr = Search(mName, parent)?.mPtr!;
      Assert.IsNotNull(mPtr, $"{mName} not found in {parent.mPtr}");
      base.SetProps(reference.GetValue());
    }
  }
}
