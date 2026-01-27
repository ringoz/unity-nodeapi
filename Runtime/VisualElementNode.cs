/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using Microsoft.JavaScript.NodeApi;

#nullable enable

[JSExport]
public class RoutedEvent : Event
{
  internal RoutedEvent() { }
  EventBase? mEvent;

  internal static readonly RoutedEvent Pool = new();
  internal RoutedEvent Reset(EventBase? evt = null)
  {
    mEvent = evt;
    return this;
  }

  public override void Dispose() => Reset();
  public override string Type => mEvent!.GetType().Name;
  public override long Timestamp => mEvent!.timestamp;
  public override object Target => mEvent!.target;
  public object CurrentTarget => mEvent!.currentTarget;
  public bool Bubbles => mEvent!.bubbles;
  public bool IsPropagationStopped => mEvent!.isPropagationStopped;
  public void StopPropagation() => mEvent!.StopPropagation();
  public void StopImmediatePropagation() => mEvent!.StopImmediatePropagation();
}

class VisualElementNode : Node
{
  sealed class EventProperty<TEventType> : Property<VisualElement, Action<Event>> where TEventType : EventBase<TEventType>, new()
  {
    public override string Name { get; } = $"on{typeof(TEventType).Name.Replace("Event", "")}";
    public override bool IsReadOnly => false;
    public override Action<Event> GetValue(ref VisualElement container) => container.GetEventHandler<TEventType>()!;
    public override void SetValue(ref VisualElement container, Action<Event> value) => container.SetEventHandler<TEventType>(value);
  }

  static VisualElementNode()
  {
    var bag = (ContainerPropertyBagEx<VisualElement>)PropertyBag.GetPropertyBag<VisualElement>();
    bag.AddProperty(new EventProperty<ClickEvent>());
  }

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

public static class VisualElementExtensions
{
  public static Dictionary<object, object> GetUserProps(this VisualElement element)
  {
    element.userData ??= new Dictionary<object, object>();
    return (Dictionary<object, object>)element.userData;
  }

  sealed class EventHandler<TEventType> where TEventType : EventBase<TEventType>, new()
  {
    public Action<Event>? handler;
    public void Invoke(TEventType evt) => Node.InvokeHandler(handler, RoutedEvent.Pool.Reset(evt));
  }

  public static Action<Event>? GetEventHandler<TEventType>(this VisualElement element) where TEventType : EventBase<TEventType>, new()
  {
    var props = element.GetUserProps();
    if (!props.TryGetValue(typeof(TEventType), out var prop))
      return null;

    var callback = (EventHandler<TEventType>)prop;
    return callback.handler;
  }

  public static void SetEventHandler<TEventType>(this VisualElement element, Action<Event>? handler) where TEventType : EventBase<TEventType>, new()
  {
    var props = element.GetUserProps();
    if (!props.TryGetValue(typeof(TEventType), out var prop))
      props.Add(typeof(TEventType), prop = new EventHandler<TEventType>());

    var callback = (EventHandler<TEventType>)prop;
    callback.handler = handler;

    if (handler != null)
      element.RegisterCallback<TEventType>(callback.Invoke);
    else
      element.UnregisterCallback<TEventType>(callback.Invoke);
  }
}
