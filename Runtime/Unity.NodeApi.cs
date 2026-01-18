/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

  protected static readonly ConditionalWeakTable<object, Node> Wrappers = new();

  internal object mPtr;
  public virtual object Ptr => mPtr;

  protected Node(object ptr) => mPtr = ptr;
  public virtual void Dispose() => (mPtr as IDisposable)?.Dispose();

  public override bool Equals(object other) => (GetType() == other.GetType()) ? Equals(mPtr, ((Node)other).mPtr) : base.Equals(other);
  public override int GetHashCode() => mPtr.GetHashCode();
  public override string ToString() => PropertiezDump.ToString(mPtr);

  private static PropertyPath PropPath(in JSValue key) => new PropertyPath(((string)key).Replace('-', '.'));
  private void SetProp(in PropertyPath key, in JSValue val)
  {
    switch (val.TypeOf())
    {
      case JSValueType.String:
        if (!PropertyContainer.TrySetValue(mPtr, key, (string)val))
          PropertyContainer.SetValue(mPtr, key, val);
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

  private static Node CreateImpl(object kind) => AttributeOverridesNode.Create(kind) ?? VisualElementNode.Create(kind) ?? ComponentNode.Create(kind) ?? GameObjectNode.Create(kind);
  public static Node Create(object kind) => CreateImpl(kind is string path ? Resources.Load(path) ?? kind : kind);
  public static Node Search(object name, Node scope = null) => scope is VisualElementNode root ? VisualElementNode.Find(name, root) : scope is GameObjectNode gobj ? ComponentNode.Find(name, gobj) : GameObjectNode.Find((string)name);

  public static Loader LoadAssetAsync { get; set; } = async (string path) =>
  {
    var request = Resources.LoadAsync(path);
    await request;
    return request.asset;
  };
}

class AttributeOverridesNode : Node
{
  protected object mName;

  public override object Ptr => mPtr is JSReference ? null : mPtr;

  protected AttributeOverridesNode(object obj) : base(obj) { }

  public static new Node Create(object kind) => kind switch
  {
    string name => name.StartsWith("#") ? new AttributeOverridesNode(null) { mName = name.Substring(1) } : null,
    _ => null
  };

  public override void Dispose() => (mPtr as JSReference)?.Dispose();

  public override void SetProps(in JSValue props)
  {
    if (mPtr is JSReference reference)
    {
      reference.Dispose();
      mPtr = null;
    }

    if (mPtr == null)
      mPtr = new JSReference(props);
    else
      base.SetProps(props);
  }

  public override void SetParent(Node parent, Node beforeChild = null)
  {
    if (parent == null)
    {
      mPtr = null;
      return;
    }

    using (var reference = (JSReference)mPtr)
    {
      mPtr = Search(mName, parent)?.mPtr;
      Assert.IsNotNull(mPtr, $"{mName} not found in {parent.mPtr}");
      base.SetProps(reference.GetValue());
    }
  }
}

class GameObjectNode : Node
{
  protected GameObjectNode(object ptr) : base(ptr) { }

  public static Node Wrap(GameObject obj) => obj ? (GameObjectNode)Wrappers.GetValue(obj, (obj) => new GameObjectNode(obj)) : null;
  public static Node Find(string name) => Wrap(GameObject.Find(name));

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

  public override void SetParent(Node parent, Node beforeChild = null)
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

class ComponentNode : AttributeOverridesNode
{
  protected ComponentNode(object obj) : base(obj) { }

  public static Node Wrap(Component obj) => obj != null ? Wrappers.GetValue(obj, (obj) => new ComponentNode(obj) { mName = obj.GetType() }) : null;
  public static Node Find(object kind, GameObjectNode scope) => Find(ParseType(kind), scope);
  public static Node Find(Type type, GameObjectNode scope) => Wrap(((GameObject)scope.mPtr).GetComponent(type) ?? ((GameObject)scope.mPtr).AddComponent(type));

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
    if (parent == null && mPtr is Transform transform)
    {
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
      transform.localScale = Vector3.one;
      return;
    }
    base.SetParent(parent, beforeChild);
  }
}

class VisualElementNode : Node
{
  protected VisualElementNode(object ptr) : base(ptr) { }

  public static Node Wrap(VisualElement obj) => obj != null ? Wrappers.GetValue(obj, (obj) => new VisualElementNode(obj)) : null;
  public static Node Find(object name, VisualElementNode scope) => Wrap(((VisualElement)scope.mPtr).Query(name.ToString()));

  private static IEnumerable<Type> Types => PropertyBag.GetAllTypesWithAPropertyBag().Where(type => typeof(VisualElement).IsAssignableFrom(type));
  private static Type ParseType(object kind) => kind as Type ?? Types.FirstOrDefault(type => type.Name == kind.ToString());

  public static new Node Create(object kind)
  {
    if (kind is VisualTreeAsset uxml)
      return new VisualElementNode(uxml.Instantiate());

    Type type = ParseType(kind);
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

  public override DOMRect GetBoundingClientRect()
  {
    Rect rc = ((VisualElement)mPtr).worldBound;
    return new DOMRect() { x = rc.xMin, y = rc.yMin, width = rc.width, height = rc.height };
  }
}
