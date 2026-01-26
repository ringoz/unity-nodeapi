/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

#nullable enable

class VisualElementNode : Node
{
  protected VisualElementNode(object ptr) : base(ptr) { }

  public static Node? Wrap(VisualElement? obj) => obj != null ? Wrappers.GetValue(obj, obj => new VisualElementNode(obj)) : null;
  public static Node? Find(object name, Node scope) => Wrap(((VisualElement)scope.mPtr).Query(name.ToString()));

  private static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(VisualElement).IsAssignableFrom(type));
  private static Type? ParseType(object kind) => kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());

  public static new Node? Create(object kind)
  {
    if (kind is VisualTreeAsset uxml)
      return new VisualElementNode(uxml.Instantiate());

    Type? type = ParseType(kind);
    return type != null ? new VisualElementNode((VisualElement)Activator.CreateInstance(type)) : null;
  }

  public override void SetActive(bool value)
  {
    ((VisualElement)mPtr).visible = value;
  }

  public override void SetParent(Node? parent, Node? beforeChild)
  {
    if (parent == null)
    {
      ((VisualElement)mPtr).parent.Remove((VisualElement)mPtr);
      return;
    }

    var parentElement = parent.mPtr as VisualElement;
    if (parentElement == null)
    {
      var parentGameObject = (GameObject)parent.mPtr;
      parentElement = parentGameObject.GetComponent<UIDocument>().rootVisualElement;
    }

    if (beforeChild?.mPtr is VisualElement beforeChildElement)
      parentElement.Insert(parentElement.IndexOf(beforeChildElement), (VisualElement)mPtr);
    else
      parentElement.Add((VisualElement)mPtr);
  }

  public override void Clear()
  {
    ((VisualElement)mPtr).Clear();
  }

  public override DOMRect? GetBoundingClientRect()
  {
    Rect rc = ((VisualElement)mPtr).worldBound;
    return new DOMRect() { x = rc.xMin, y = rc.yMin, width = rc.width, height = rc.height };
  }
}
