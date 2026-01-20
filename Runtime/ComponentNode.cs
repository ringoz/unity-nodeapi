/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;

class ComponentNode : AttributeOverridesNode
{
  protected ComponentNode(object obj) : base(obj) { }

  public static Node Wrap(Component obj) => obj != null ? Wrappers.GetValue(obj, obj => new ComponentNode(obj) { mName = obj.GetType() }) : null;
  public static Node Find(Type type, Node scope) => Wrap(((GameObject)scope.mPtr).GetOrAddComponent(type));
  public static Node Find(object kind, Node scope) => Find(ParseType(kind), scope);

  private static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(Component).IsAssignableFrom(type));
  private static Type ParseType(object kind) => kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());

  public static new Node Create(object kind)
  {
    Type type = ParseType(kind);
    return type != null ? new ComponentNode(null) { mName = type } : null;
  }

  public override void SetActive(bool value)
  {
    if (mPtr is Behaviour)
      ((Behaviour)mPtr).enabled = value;
    else
      base.SetActive(value);
  }

  public override void SetParent(Node parent, Node beforeChild = null)
  {
    if (parent == null)
    {
      Wrappers.Remove(mPtr);
      if (mPtr is Transform transform)
      {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
      }
    }
    
    base.SetParent(parent, beforeChild);
    if (parent != null)
      Wrappers.AddOrUpdate(mPtr, this);
  }
}
