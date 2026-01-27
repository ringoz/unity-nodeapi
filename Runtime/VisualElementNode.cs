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
  public RoutedEvent() { }

  protected EventBase? mEvent;
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

[JSExport]
public class PointerEvent : RoutedEvent
{
  public PointerEvent() { }

  public int pointerId => ((IPointerEvent)mEvent!).pointerId;
  public string pointerType => ((IPointerEvent)mEvent!).pointerType;
  public bool isPrimary => ((IPointerEvent)mEvent!).isPrimary;
  public int button => ((IPointerEvent)mEvent!).button;
  public int pressedButtons => ((IPointerEvent)mEvent!).pressedButtons;
  public float[] position => ((IPointerEvent)mEvent!).position.ToArray();
  public float[] localPosition => ((IPointerEvent)mEvent!).localPosition.ToArray();
  public float[] deltaPosition => ((IPointerEvent)mEvent!).deltaPosition.ToArray();
  public float deltaTime => ((IPointerEvent)mEvent!).deltaTime;
  public int clickCount => ((IPointerEvent)mEvent!).clickCount;
  public float pressure => ((IPointerEvent)mEvent!).pressure;
  public float tangentialPressure => ((IPointerEvent)mEvent!).tangentialPressure;
  public float altitudeAngle => ((IPointerEvent)mEvent!).altitudeAngle;
  public float azimuthAngle => ((IPointerEvent)mEvent!).azimuthAngle;
  public float twist => ((IPointerEvent)mEvent!).twist;
  public float[] tilt => ((IPointerEvent)mEvent!).tilt.ToArray();
  public float[] radius => ((IPointerEvent)mEvent!).radius.ToArray();
  public float[] radiusVariance => ((IPointerEvent)mEvent!).radiusVariance.ToArray();
  public bool shiftKey => ((IPointerEvent)mEvent!).shiftKey;
  public bool ctrlKey => ((IPointerEvent)mEvent!).ctrlKey;
  public bool commandKey => ((IPointerEvent)mEvent!).commandKey;
  public bool altKey => ((IPointerEvent)mEvent!).altKey;
  public bool actionKey => ((IPointerEvent)mEvent!).actionKey;
}

class VisualElementNode : Node
{
  sealed class EventProperty<TEvent, TEventType> : Property<VisualElement, Action<TEvent>>
    where TEvent : RoutedEvent, new()
    where TEventType : EventBase<TEventType>, new()
  {
    static readonly TEvent Pool = new TEvent();

    sealed class Handler
    {
      internal Action<TEvent>? callback;
      internal void Invoke(TEventType evt) => InvokeHandler(callback, (TEvent)Pool.Reset(evt));

      internal static Action<TEvent>? Get(VisualElement element)
      {
        var props = element.GetUserProps();
        if (!props.TryGetValue(typeof(TEventType), out var prop))
          return null;

        var handler = (Handler)prop;
        return handler.callback;
      }

      internal static void Set(VisualElement element, Action<TEvent>? callback)
      {
        var props = element.GetUserProps();
        if (!props.TryGetValue(typeof(TEventType), out var prop))
          props.Add(typeof(TEventType), prop = new Handler());

        var handler = (Handler)prop;
        if ((handler.callback = callback) != null)
          element.RegisterCallback<TEventType>(handler.Invoke);
        else
          element.UnregisterCallback<TEventType>(handler.Invoke);
      }
    }

    public override string Name { get; } = $"on{typeof(TEventType).Name.Replace("Event", "")}";
    public override bool IsReadOnly => false;
    public override Action<TEvent> GetValue(ref VisualElement container) => Handler.Get(container)!;
    public override void SetValue(ref VisualElement container, Action<TEvent> value) => Handler.Set(container, value);
  }

  static VisualElementNode()
  {
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.ToAction<RoutedEvent>());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.ToAction<PointerEvent>());

    var bag = (ContainerPropertyBagEx<VisualElement>)PropertyBag.GetPropertyBag<VisualElement>();
    bag.AddProperty(new EventProperty<PointerEvent, ClickEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerCaptureEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerCaptureOutEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerDownEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerUpEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerMoveEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerEnterEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerLeaveEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerOverEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerOutEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, PointerCancelEvent>());
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
}
