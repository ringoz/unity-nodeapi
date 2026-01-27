/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using UnityEngine;
using Unity.Properties;

#nullable enable

class GameObjectEvent : Event
{
  GameObject? mTarget;

  internal static readonly GameObjectEvent Pool = new();
  internal GameObjectEvent Reset(GameObject? target = null)
  {
    mTarget = target;
    return this;
  }

  public override void Dispose() => Reset();
  public override object Target => mTarget!;
}

class GameObjectNode : Node
{
  abstract class EventBehaviour : MonoBehaviour
  {
    internal Action<Event> handler;
    internal void Invoke() => InvokeHandler(handler, GameObjectEvent.Pool.Reset(gameObject));
  }

  sealed class EventProperty<TBehaviour> : Property<GameObject, Action<Event>> where TBehaviour : EventBehaviour
  {
    public override string Name => typeof(TBehaviour).Name;
    public override bool IsReadOnly => false;
    public override Action<Event> GetValue(ref GameObject container) => container.GetComponent<TBehaviour>()?.handler!;
    public override void SetValue(ref GameObject container, Action<Event> value) => container.GetOrAddComponent<TBehaviour>().handler = value;
  }

  sealed class onAwake : EventBehaviour { void Awake() => Invoke(); }
  sealed class onStart : EventBehaviour { void Start() => Invoke(); }
  sealed class onUpdate : EventBehaviour { void Update() => Invoke(); }
  sealed class onFixedUpdate : EventBehaviour { void FixedUpdate() => Invoke(); }
  sealed class onLateUpdate : EventBehaviour { void LateUpdate() => Invoke(); }
  sealed class onDestroy : EventBehaviour { void OnDestroy() => Invoke(); }
  sealed class onEnable : EventBehaviour { void OnEnable() => Invoke(); }
  sealed class onDisable : EventBehaviour { void OnDisable() => Invoke(); }
  sealed class onBecameInvisible : EventBehaviour { void OnBecameInvisible() => Invoke(); }
  sealed class onBecameVisible : EventBehaviour { void OnBecameVisible() => Invoke(); }
  sealed class onMouseDown : EventBehaviour { void OnMouseDown() => Invoke(); }
  sealed class onMouseDrag : EventBehaviour { void OnMouseDrag() => Invoke(); }
  sealed class onMouseEnter : EventBehaviour { void OnMouseEnter() => Invoke(); }
  sealed class onMouseExit : EventBehaviour { void OnMouseExit() => Invoke(); }
  sealed class onMouseOver : EventBehaviour { void OnMouseOver() => Invoke(); }
  sealed class onMouseUp : EventBehaviour { void OnMouseUp() => Invoke(); }
  sealed class onMouseUpAsButton : EventBehaviour { void OnMouseUpAsButton() => Invoke(); }

  static GameObjectNode()
  {
    var bag = (ContainerPropertyBagEx<GameObject>)PropertyBag.GetPropertyBag<GameObject>();
    bag.AddProperty(new EventProperty<onAwake>());
    bag.AddProperty(new EventProperty<onStart>());
    bag.AddProperty(new EventProperty<onUpdate>());
    bag.AddProperty(new EventProperty<onFixedUpdate>());
    bag.AddProperty(new EventProperty<onLateUpdate>());
    bag.AddProperty(new EventProperty<onDestroy>());
    bag.AddProperty(new EventProperty<onEnable>());
    bag.AddProperty(new EventProperty<onDisable>());
    bag.AddProperty(new EventProperty<onBecameInvisible>());
    bag.AddProperty(new EventProperty<onBecameVisible>());
    bag.AddProperty(new EventProperty<onMouseDown>());
    bag.AddProperty(new EventProperty<onMouseDrag>());
    bag.AddProperty(new EventProperty<onMouseEnter>());
    bag.AddProperty(new EventProperty<onMouseExit>());
    bag.AddProperty(new EventProperty<onMouseOver>());
    bag.AddProperty(new EventProperty<onMouseUp>());
    bag.AddProperty(new EventProperty<onMouseUpAsButton>());
  }

  protected GameObjectNode(object ptr) : base(ptr) { }

  public static Node? Wrap(GameObject? obj) => obj != null ? Wrappers.GetValue(obj, obj => new GameObjectNode(obj)) : null;
  public static Node? Find(string name) => Wrap(GameObject.Find(name));

  private static GameObjectNode _null = new GameObjectNode(null!);
  private static GameObjectNode Null
  {
    get
    {
      if (_null.mPtr == null)
      {
        _null.mPtr = new GameObject();
        _null.SetActive(false);
      }
      return _null;
    }
  }

  public static new Node? Create(object kind)
  {
    if (kind is GameObject prefab)
      return Wrap(UnityEngine.Object.Instantiate(prefab, ((GameObject)Null.mPtr).transform, false));
    return null;
  }

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((GameObject)mPtr);
  }

  public override void SetActive(bool value)
  {
    ((GameObject)mPtr).SetActive(value);
  }

  public override void SetParent(Node? parent, Node? beforeChild)
  {
    var parentGameObject = (GameObject)(parent ?? Null).mPtr;
    ((GameObject)mPtr).transform.SetParent(parentGameObject.transform, false);
    if (beforeChild?.mPtr is GameObject beforeChildGameObject)
      ((GameObject)mPtr).transform.SetSiblingIndex(beforeChildGameObject.transform.GetSiblingIndex());
  }

  public override void Clear()
  {
    foreach (Transform child in ((GameObject)mPtr).transform)
      child.SetParent(((GameObject)Null.mPtr).transform, false);
  }

  public override DOMRect? GetBoundingClientRect()
  {
    var renderer = ((GameObject)mPtr).GetComponent<Renderer>();
    if (renderer == null)
      return base.GetBoundingClientRect();

    var c = renderer.bounds.center;
    var e = renderer.bounds.extents;
    var corners = new[]
    {
      new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
    }.Select(corner => Camera.main.WorldToScreenPoint(corner));

    var maxX = corners.Max(corner => corner.x) * 96 / Screen.dpi;
    var minX = corners.Min(corner => corner.x) * 96 / Screen.dpi;
    var maxY = corners.Max(corner => Screen.height - corner.y) * 96 / Screen.dpi;
    var minY = corners.Min(corner => Screen.height - corner.y) * 96 / Screen.dpi;
    return new DOMRect() { x = minX, y = minY, width = maxX - minX, height = maxY - minY };
  }
}

public static class GameObjectExtensions
{
  public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
  {
    if (!gameObject.TryGetComponent<T>(out T component))
      component = gameObject.AddComponent<T>();
    return component;
  }

  public static Component GetOrAddComponent(this GameObject gameObject, Type type)
  {
    if (!gameObject.TryGetComponent(type, out Component component))
      component = gameObject.AddComponent(type);
    return component;
  }
}
