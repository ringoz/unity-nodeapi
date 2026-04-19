/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/
#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;

[assembly: GeneratePropertyBagsForType(typeof(Component))]
[assembly: GeneratePropertyBagsForType(typeof(Transform))]
[assembly: GeneratePropertyBagsForType(typeof(Behaviour))]
[assembly: GeneratePropertyBagsForType(typeof(MonoBehaviour))]

class ComponentNode : AttributeOverridesNode
{
  protected ComponentNode(object obj) : base(obj) { }

  public static Node? Wrap(Component? obj) => Wrap(obj, obj => new ComponentNode(obj) { mName = obj.GetType() });
  public static Node? Find(Type type, Node scope) => Wrap(((GameObject)scope.mPtr).GetOrAddComponent(type));
  public static Node? Find(object kind, Node scope) => Find(ParseType(kind)!, scope);
  
  public static IEnumerable<Node> Enum(Node parent)
  {
    if (parent.mPtr is GameObject)
      return ((GameObject)parent.mPtr).GetComponents<Component>().Select(child => Wrap(child)!);
      
    return Enumerable.Empty<Node>();
  }

  private static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(Component).IsAssignableFrom(type));
  private static Type? ParseType(object kind) => kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());

  public static new Node? Create(object kind)
  {
    Type? type = ParseType(kind);
    return type != null ? new ComponentNode(null!) { mName = type } : null;
  }

  public override void SetActive(bool value)
  {
    if (mPtr is Behaviour)
      ((Behaviour)mPtr).enabled = value;
    else
      base.SetActive(value);
  }

  public override void SetParent(Node? parent, Node? beforeChild)
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
