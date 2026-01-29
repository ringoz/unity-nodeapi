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
  protected EventBase? mEvent;
  internal RoutedEvent Reset(EventBase? evt = null)
  {
    mEvent = evt;
    return this;
  }

  public override void Dispose() => Reset();
  public override string Type => mEvent!.GetType().Name;
  public override long Timestamp => mEvent!.timestamp;
  public override Node Target => VisualElementNode.Wrap((VisualElement)mEvent!.target)!;
  public Node CurrentTarget => VisualElementNode.Wrap((VisualElement)mEvent!.currentTarget)!;
  public Node? RelatedTarget => VisualElementNode.Wrap((VisualElement?)(mEvent as IFocusEvent)?.relatedTarget);
  public bool Bubbles => mEvent!.bubbles;
  public bool IsPropagationStopped => mEvent!.isPropagationStopped;
  public void StopPropagation() => mEvent!.StopPropagation();
  public void StopImmediatePropagation() => mEvent!.StopImmediatePropagation();
}

[JSExport]
public class ChangeEvent : RoutedEvent
{
  public bool OldBoolean => ((ChangeEvent<bool>)mEvent!).previousValue;
  public bool NewBoolean => ((ChangeEvent<bool>)mEvent!).newValue;
  public int OldInt32 => ((ChangeEvent<int>)mEvent!).previousValue;
  public int NewInt32 => ((ChangeEvent<int>)mEvent!).newValue;
  public float OldSingle => ((ChangeEvent<float>)mEvent!).previousValue;
  public float NewSingle => ((ChangeEvent<float>)mEvent!).newValue;
  public string OldString => (mEvent as InputEvent)?.previousData ?? ((ChangeEvent<string>)mEvent!).previousValue;
  public string NewString => (mEvent as InputEvent)?.newData ?? ((ChangeEvent<string>)mEvent!).newValue;
  public float[] OldRect => (mEvent as GeometryChangedEvent)?.oldRect.AsEnumerable().ToArray() ?? ((ChangeEvent<Rect>)mEvent!).previousValue.AsEnumerable().ToArray();
  public float[] NewRect => (mEvent as GeometryChangedEvent)?.newRect.AsEnumerable().ToArray() ?? ((ChangeEvent<Rect>)mEvent!).newValue.AsEnumerable().ToArray();
}

[JSExport]
public class KeyboardEvent : RoutedEvent
{
  public string Character => ((IKeyboardEvent)mEvent!).character.ToString();
  public int KeyCode => (int)((IKeyboardEvent)mEvent!).keyCode;
  public bool ShiftKey => ((IKeyboardEvent)mEvent!).shiftKey;
  public bool CtrlKey => ((IKeyboardEvent)mEvent!).ctrlKey;
  public bool CommandKey => ((IKeyboardEvent)mEvent!).commandKey;
  public bool AltKey => ((IKeyboardEvent)mEvent!).altKey;
  public bool ActionKey => ((IKeyboardEvent)mEvent!).actionKey;
}

[JSExport]
public class PointerEvent : RoutedEvent
{
  public int PointerId => (mEvent as IPointerEvent)?.pointerId ?? UnityEngine.UIElements.PointerId.mousePointerId;
  public string PointerType => (mEvent as IPointerEvent)?.pointerType ?? "mouse";
  public bool IsPrimary => (mEvent as IPointerEvent)?.isPrimary ?? true;
  public int Button => (mEvent as IPointerEvent)?.button ?? ((IMouseEvent)mEvent!).button;
  public int PressedButtons => (mEvent as IPointerEvent)?.pressedButtons ?? ((IMouseEvent)mEvent!).pressedButtons;
  public float[] Position => (mEvent as IPointerEvent)?.position.AsEnumerable().ToArray() ?? ((IMouseEvent)mEvent!).mousePosition.AsEnumerable().ToArray();
  public float[] LocalPosition => (mEvent as IPointerEvent)?.localPosition.AsEnumerable().ToArray() ?? ((IMouseEvent)mEvent!).localMousePosition.AsEnumerable().ToArray();
  public float[] DeltaPosition => (mEvent as IPointerEvent)?.deltaPosition.AsEnumerable().ToArray() ?? ((IMouseEvent)mEvent!).mouseDelta.AsEnumerable().ToArray();
  public float[] WheelDelta => (mEvent as WheelEvent)?.delta.AsEnumerable().ToArray() ?? Vector3.zero.AsEnumerable().ToArray();
  public float DeltaTime => (mEvent as IPointerEvent)?.deltaTime ?? 0;
  public int ClickCount => (mEvent as IPointerEvent)?.clickCount ?? ((IMouseEvent)mEvent!).clickCount;
  public float Pressure => (mEvent as IPointerEvent)?.pressure ?? 0;
  public float TangentialPressure => (mEvent as IPointerEvent)?.tangentialPressure ?? 0;
  public float AltitudeAngle => (mEvent as IPointerEvent)?.altitudeAngle ?? 0;
  public float AzimuthAngle => (mEvent as IPointerEvent)?.azimuthAngle ?? 0;
  public float Twist => (mEvent as IPointerEvent)?.twist ?? 0;
  public float[] Tilt => (mEvent as IPointerEvent)?.tilt.AsEnumerable().ToArray() ?? Vector2.zero.AsEnumerable().ToArray();
  public float[] Radius => (mEvent as IPointerEvent)?.radius.AsEnumerable().ToArray() ?? Vector2.zero.AsEnumerable().ToArray();
  public float[] RadiusVariance => (mEvent as IPointerEvent)?.radiusVariance.AsEnumerable().ToArray() ?? Vector2.zero.AsEnumerable().ToArray();
  public bool ShiftKey => (mEvent as IPointerEvent)?.shiftKey ?? ((IMouseEvent)mEvent!).shiftKey;
  public bool CtrlKey => (mEvent as IPointerEvent)?.ctrlKey ?? ((IMouseEvent)mEvent!).ctrlKey;
  public bool CommandKey => (mEvent as IPointerEvent)?.commandKey ?? ((IMouseEvent)mEvent!).commandKey;
  public bool AltKey => (mEvent as IPointerEvent)?.altKey ?? ((IMouseEvent)mEvent!).altKey;
  public bool ActionKey => (mEvent as IPointerEvent)?.actionKey ?? ((IMouseEvent)mEvent!).actionKey;
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

    public override string Name { get; } = $"on{typeof(TEventType).Name.Replace("Event", "").Replace("`1", "")}{typeof(TEventType).GetGenericArguments().Select(t => t.Name).FirstOrDefault()}";
    public override bool IsReadOnly => false;
    public override Action<TEvent> GetValue(ref VisualElement container) => Handler.Get(container)!;
    public override void SetValue(ref VisualElement container, Action<TEvent> value) => Handler.Set(container, value);
  }

  static VisualElementNode()
  {
    TypeConversion.Register(JS((Action<RoutedEvent> v) => new JSFunction((JSValue e) => v((RoutedEvent)e.Unwrap("RoutedEvent")))));
    TypeConversion.Register(JS((Action<ChangeEvent> v) => new JSFunction((JSValue e) => v((ChangeEvent)e.Unwrap("ChangeEvent")))));
    TypeConversion.Register(JS((Action<KeyboardEvent> v) => new JSFunction((JSValue e) => v((KeyboardEvent)e.Unwrap("KeyboardEvent")))));
    TypeConversion.Register(JS((Action<PointerEvent> v) => new JSFunction((JSValue e) => v((PointerEvent)e.Unwrap("PointerEvent")))));
    
    TypeConversion.Register(JS((JSValue v) => v.ToAction<RoutedEvent>()));
    TypeConversion.Register(JS((JSValue v) => v.ToAction<ChangeEvent>()));
    TypeConversion.Register(JS((JSValue v) => v.ToAction<KeyboardEvent>()));
    TypeConversion.Register(JS((JSValue v) => v.ToAction<PointerEvent>()));

    var bag = (ContainerPropertyBagEx<VisualElement>)PropertyBag.GetPropertyBag<VisualElement>();
    bag.AddProperty(new EventProperty<RoutedEvent, AttachToPanelEvent>());
    bag.AddProperty(new EventProperty<RoutedEvent, DetachFromPanelEvent>());
    bag.AddProperty(new EventProperty<RoutedEvent, BlurEvent>());
    bag.AddProperty(new EventProperty<RoutedEvent, FocusEvent>());
    bag.AddProperty(new EventProperty<RoutedEvent, FocusOutEvent>());
    bag.AddProperty(new EventProperty<RoutedEvent, FocusInEvent>());
    bag.AddProperty(new EventProperty<ChangeEvent, ChangeEvent<bool>>());
    bag.AddProperty(new EventProperty<ChangeEvent, ChangeEvent<int>>());
    bag.AddProperty(new EventProperty<ChangeEvent, ChangeEvent<float>>());
    bag.AddProperty(new EventProperty<ChangeEvent, ChangeEvent<string>>());
    bag.AddProperty(new EventProperty<ChangeEvent, ChangeEvent<Rect>>());
    bag.AddProperty(new EventProperty<ChangeEvent, InputEvent>());
    bag.AddProperty(new EventProperty<ChangeEvent, GeometryChangedEvent>());
    bag.AddProperty(new EventProperty<KeyboardEvent, KeyDownEvent>());
    bag.AddProperty(new EventProperty<KeyboardEvent, KeyUpEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, ClickEvent>());
    bag.AddProperty(new EventProperty<PointerEvent, WheelEvent>());
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
