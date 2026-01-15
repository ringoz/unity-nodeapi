/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using Unity.Properties;

[assembly: AlwaysLinkAssembly]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.Object))]
[assembly: GeneratePropertyBagsForType(typeof(GameObject))]
[assembly: GeneratePropertyBagsForType(typeof(Component))]
[assembly: GeneratePropertyBagsForType(typeof(Transform))]
[assembly: GeneratePropertyBagsForType(typeof(Behaviour))]
[assembly: GeneratePropertyBagsForType(typeof(MonoBehaviour))]
[assembly: GeneratePropertyBagsForType(typeof(UIDocument))]
[assembly: GeneratePropertyBagsForType(typeof(VisualElement))]
[assembly: GeneratePropertyBagsForType(typeof(BindableElement))]
[assembly: GeneratePropertyBagsForType(typeof(TextElement))]

[JSExport]
public struct DOMRect
{
  public float x { get; init; }
  public float y { get; init; }
  public float width { get; init; }
  public float height { get; init; }
  public float left => x + 0.5f;
  public float top => y + 0.5f;
  public float right => left + width;
  public float bottom => top + height;
}

[JSExport]
public delegate Task<object> Loader(string path);

[JSExport]
public class Node : IDisposable
{
  static Node()
  {
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (char)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (bool)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (sbyte)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (short)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (int)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (long)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (byte)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (ushort)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (uint)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (ulong)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (float)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (double)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (string)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (object)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (int)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (float)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (string)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (object)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new PropertyPath((string)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2((float)v[0], (float)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2Int((int)v[0], (int)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3((float)v[0], (float)v[1], (float)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3Int((int)v[0], (int)v[1], (int)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Quaternion((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Matrix4x4(new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]), new Vector4((float)v[4], (float)v[5], (float)v[6], (float)v[7]), new Vector4((float)v[8], (float)v[9], (float)v[10], (float)v[11]), new Vector4((float)v[12], (float)v[13], (float)v[14], (float)v[15])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Color((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Rect((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new RectInt((int)v[0], (int)v[1], (int)v[2], (int)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Bounds(new Vector3((float)v[0], (float)v[1], (float)v[2]), new Vector3((float)v[3], (float)v[4], (float)v[5])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new BoundsInt(new Vector3Int((int)v[0], (int)v[1], (int)v[2]), new Vector3Int((int)v[3], (int)v[4], (int)v[5])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Version((int)v[0], (int)v[1], (int)v[2], (int)v[3]));
  }

  public static bool IsPropTypeSupported<T>()
  {
    var source = JSValue.Undefined;
    return TypeConversion.TryConvert(ref source, out T destination);
  }

  internal object mPtr;
  public object Ptr => mPtr;

  protected Node(object ptr) => mPtr = ptr;
  public virtual void Dispose() => (mPtr as IDisposable)?.Dispose();

  public override bool Equals(object ptr) => (ptr is Node instance) ? ptr.Equals(instance.mPtr) : base.Equals(ptr);
  public override int GetHashCode() => mPtr.GetHashCode();
  public override string ToString() => PropertiezDump.ToString(mPtr);

  static private PropertyPath PropPath(in JSValue key) => new PropertyPath(((string)key).Replace('-', '.'));
  private void SetProp(in PropertyPath key, in JSValue val)
  {
    switch (val.TypeOf())
    {
      case JSValueType.String:
        try { PropertyContainer.SetValue(mPtr, key, (string)val); }
        catch { PropertyContainer.SetValue(mPtr, key, val); }
        break;

      case JSValueType.External:
        var obj = val.TryGetValueExternal();
        if (obj is UnityEngine.Object ueo)
          PropertyContainer.SetValue(mPtr, key, ueo);
        else
          PropertyContainer.SetValue(mPtr, key, obj);
        break;

      default:
        var part = key[key.Length - 1];
        if (part.IsName && (part.Name.EndsWith("Flags") || part.Name.EndsWith("Hints")) && val.IsArray())
          PropertyContainer.SetValue(mPtr, key, string.Join(',', ((JSArray)val).Select(v => (string)v)));
        else
          PropertyContainer.SetValue(mPtr, key, val);
        break;
    }
  }

  public virtual void SetProps(in JSValue props) { foreach (var item in (JSObject)props) SetProp(PropPath(item.Key), item.Value); }
  public virtual void SetActive(bool value) => throw new NotImplementedException();
  public virtual void SetParent(Node parent, Node beforeChild = null) => throw new NotImplementedException();
  public virtual void Clear() => throw new NotImplementedException();
  public virtual DOMRect GetBoundingClientRect() => default;

  public static Node Create(object kind) => VisualElementNode.Create(kind) ?? ComponentNode.Create(kind) ?? GameObjectNode.Create(kind);
  public static Node Search(string name) => GameObjectNode.Find(name);

  public static Loader LoadAssetAsync { get; set; } = async (string path) =>
  {
    var request = Resources.LoadAsync(path);
    await request;
    return request.asset;
  };
}

class ObjectNode : Node
{
  protected ObjectNode(object ptr) : base(ptr) { }

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object)mPtr);
  }
}

class GameObjectNode : ObjectNode
{
  protected GameObjectNode(object ptr) : base(ptr) { }

  public static GameObjectNode Wrap(GameObject obj) => obj ? new GameObjectNode(obj) : null;
  public static GameObjectNode Find(string name) => Wrap(GameObject.Find(name));

  private static GameObjectNode _null = new GameObjectNode(null);
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

  public static new Node Create(object kind)
  {
    var obj = kind switch
    {
      GameObject prefab => prefab,
      string path => Resources.Load<GameObject>(path),
      _ => null
    };
    return obj ? Wrap(UnityEngine.Object.Instantiate(obj, ((GameObject)Null.mPtr).transform, false)) : null;
  }

  public override void SetActive(bool value)
  {
    ((GameObject)mPtr).SetActive(value);
  }

  public override void SetParent(Node parent, Node beforeChild = null)
  {
    if (parent == null)
      parent = Null;

    if (parent.mPtr is GameObject)
    {
      ((GameObject)mPtr).transform.SetParent(((GameObject)parent.mPtr).transform, false);
      if (beforeChild is GameObjectNode)
        ((GameObject)mPtr).transform.SetSiblingIndex(((GameObject)beforeChild.mPtr).transform.GetSiblingIndex());
    }
    else
      base.SetParent(parent, beforeChild);
  }

  public override void Clear()
  {
    foreach (Transform child in ((GameObject)mPtr).transform)
      child.SetParent(((GameObject)Null.mPtr).transform, false);
  }

  public override DOMRect GetBoundingClientRect()
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

class ComponentNode : ObjectNode
{
  protected ComponentNode(Type type) : base(new List<object> { type }) { }

  public static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(Component).IsAssignableFrom(type));
  public static new Node Create(object kind)
  {
    Type type = kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());
    return type != null ? new ComponentNode(type) : null;
  }

  public override void Dispose()
  {
    // Destroying the transform component is not allowed.
    if (mPtr is Transform transform)
    {
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
      transform.localScale = Vector3.one;
      return;
    }

    base.Dispose();
  }

  public override void SetProps(in JSValue props)
  {
    if (mPtr is List<object> list)
      list.Add(new JSReference(props));
    else
      base.SetProps(props);
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
      Dispose();
      return;
    }

    if (parent.mPtr is GameObject && mPtr is List<object> list)
    {
      var type = (Type)list.First();
      mPtr = ((GameObject)parent.mPtr).GetComponent(type) ?? ((GameObject)parent.mPtr).AddComponent(type);
      foreach (var props in list.Skip(1))
        using (var reference = (JSReference)props)
          base.SetProps(reference.GetValue());
    }
    else
      base.SetParent(parent, beforeChild);
  }
}

class VisualElementNode : Node
{
  protected VisualElementNode(object ptr) : base(ptr) { }

  public static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(VisualElement).IsAssignableFrom(type));
  public static new Node Create(object kind)
  {
    Type type = kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());
    return type != null ? new VisualElementNode((VisualElement)Activator.CreateInstance(type)) : null;
  }

  public override void SetActive(bool value)
  {
    ((VisualElement)mPtr).visible = value;
  }

  public override void SetParent(Node parent, Node beforeChild = null)
  {
    if (parent == null)
    {
      ((VisualElement)mPtr).parent.Remove((VisualElement)mPtr);
    }
    else if (parent.mPtr is GameObject parentGameObject)
    {
      parentGameObject.GetComponent<UIDocument>().rootVisualElement.Add((VisualElement)mPtr);
    }
    else if (parent.mPtr is VisualElement parentElement)
    {
      if (beforeChild.mPtr is VisualElement beforeChildElement)
        parentElement.Insert(parentElement.IndexOf(beforeChildElement), (VisualElement)mPtr);
      else
        parentElement.Add((VisualElement)mPtr);
    }
    else
      base.SetParent(parent, beforeChild);
  }

  public override void Clear()
  {
    ((VisualElement)mPtr).Clear();
  }

  public override DOMRect GetBoundingClientRect()
  {
    Rect rc = ((VisualElement)mPtr).worldBound;
    return new DOMRect() { x = rc.xMin, y = rc.yMin, width = rc.width, height = rc.height };
  }
}
