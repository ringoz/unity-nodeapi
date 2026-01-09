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
using Unity.Properties;

[assembly: AlwaysLinkAssembly]
[assembly: GeneratePropertyBagsForType(typeof(GameObject))]
[assembly: GeneratePropertyBagsForType(typeof(Transform))]
[assembly: GeneratePropertyBagsForType(typeof(Behaviour))]

static class Extensions
{
  public static string Uncapitalize(this string s) => char.ToLower(s[0]) + s.Substring(1);
}

[JSExport]
public struct Rect
{
  public float x { get; init; }
  public float y { get; init; }
  public float width { get; init; }
  public float height { get; init; }
  public float left => x;
  public float top => y;
  public float right => x + width;
  public float bottom => y + height;
}

[JSExport]
public delegate Task<object> Loader(string path);

[JSExport]
public class Element : IDisposable
{
  static Element()
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
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2Int((int)v[0], (int)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3Int((int)v[0], (int)v[1], (int)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2((float)v[0], (float)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3((float)v[0], (float)v[1], (float)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Quaternion((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
  }

  internal object mObj;

  protected Element(object obj) => mObj = obj;
  public virtual void Dispose() => (mObj as IDisposable)?.Dispose();

  public override bool Equals(object obj) => (obj is Element instance) ? obj.Equals(instance.mObj) : base.Equals(obj);
  public override int GetHashCode() => mObj.GetHashCode();
  public override string ToString() => PropertiezDump.ToString(mObj);

  public virtual void SetProps(JSValue props) { foreach (var item in (JSObject)props) PropertyContainer.SetValue(mObj, (string)item.Key, item.Value); }
  public virtual void SetActive(bool value) => throw new NotImplementedException();
  public virtual void SetParent(Element parent, Element beforeChild = null) => throw new NotImplementedException();
  public virtual void Clear() => throw new NotImplementedException();
  public virtual Rect GetBoundingClientRect() => default;

  public static Element Create(object kind) => ComponentElement.Create(kind) ?? GameObjectElement.Create(kind);
  public static Element Search(string name) => GameObjectElement.Find(name);

  public static Loader LoadAssetAsync { get; set; } = async (string path) =>
  {
    var request = Resources.LoadAsync(path);
    await request;
    return request.asset;
  };
}

class ObjectElement : Element
{
  protected ObjectElement(object obj) : base(obj) { }

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object)mObj);
  }
}

class GameObjectElement : ObjectElement
{
  protected GameObjectElement(object obj) : base(obj) { }

  public static GameObjectElement Wrap(GameObject obj) => obj ? new GameObjectElement(obj) : null;
  public static GameObjectElement Find(string name) => Wrap(GameObject.Find(name));

  private static GameObjectElement _null = new GameObjectElement(null);
  private static GameObjectElement Null
  {
    get
    {
      if (_null.mObj == null)
      {
        _null.mObj = new GameObject();
        _null.SetActive(false);
      }
      return _null;
    }
  }

  public static new Element Create(object kind)
  {
    var obj = kind switch
    {
      GameObject prefab => prefab,
      string path => Resources.Load<GameObject>(path),
      _ => null
    };
    return obj ? Wrap(UnityEngine.Object.Instantiate(obj, ((GameObject)Null.mObj).transform, false)) : null;
  }

  public override void SetActive(bool value)
  {
    ((GameObject)mObj).SetActive(value);
  }

  public override void SetParent(Element parent, Element beforeChild = null)
  {
    if (parent == null)
      parent = Null;

    if (parent.mObj is GameObject)
    {
      ((GameObject)mObj).transform.SetParent(((GameObject)parent.mObj).transform, false);
      if (beforeChild is GameObjectElement)
        ((GameObject)mObj).transform.SetSiblingIndex(((GameObject)beforeChild.mObj).transform.GetSiblingIndex());
    }
    else
      base.SetParent(parent, beforeChild);
  }

  public override void Clear()
  {
    foreach (Transform child in ((GameObject)mObj).transform)
      child.SetParent(((GameObject)Null.mObj).transform, false);
  }

  public override Rect GetBoundingClientRect()
  {
    var renderer = ((GameObject)mObj).GetComponent<Renderer>();
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
    return new Rect() { x = minX, y = minY, width = maxX - minX, height = maxY - minY };
  }
}

class ComponentElement : ObjectElement
{
  public static IDictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

  static ComponentElement()
  {
    var types = PropertyBag.GetAllTypesWithAPropertyBag();
    foreach (var type in types.Where(type => type.IsSubclassOf(typeof(Component))))
      Types.Add(KeyValuePair.Create(type.Name.Uncapitalize(), type));
  }

  protected ComponentElement(Type type) : base(new List<object> { type }) { }

  public static new Element Create(object kind) => kind switch
  {
    Type type => new ComponentElement(type),
    string path => Types.TryGetValue(path, out Type type) ? new ComponentElement(type) : null,
    _ => null
  };

  public override void Dispose()
  {
    // Destroying the transform component is not allowed.
    if (mObj is Transform transform)
    {
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
      transform.localScale = Vector3.one;
      return;
    }

    base.Dispose();
  }

  public override void SetProps(JSValue props)
  {
    if (mObj is List<object> refs)
      refs.Add(new JSReference(props));
    else
      base.SetProps(props);
  }

  public override void SetActive(bool value)
  {
    if (mObj is Behaviour)
      ((Behaviour)mObj).enabled = value;
    else
      base.SetActive(value);
  }

  public override void SetParent(Element parent, Element beforeChild = null)
  {
    if (parent == null)
    {
      Dispose();
      return;
    }

    if (parent.mObj is GameObject && mObj is List<object> refs)
    {
      var type = (Type)refs.First();
      mObj = ((GameObject)parent.mObj).GetComponent(type) ?? ((GameObject)parent.mObj).AddComponent(type);
      foreach (var prop in refs.Skip(1))
        using (var reference = (JSReference)prop)
          base.SetProps(reference.GetValue());
    }
    else
      base.SetParent(parent, beforeChild);
  }
}
