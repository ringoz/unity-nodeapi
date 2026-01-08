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
public class Element : IDisposable
{
  internal object mObj;

  protected Element(object obj) => mObj = obj;
  public virtual void Dispose() => (mObj as IDisposable)?.Dispose();

  public override bool Equals(object obj) => (obj is Element instance) ? obj.Equals(instance.mObj) : base.Equals(obj);
  public override int GetHashCode() => mObj.GetHashCode();
  public override string ToString() => PropertiezDump.ToString(mObj);

  public virtual void SetProperty(string key, object value) => PropertyContainer.SetValue(mObj, key, value?.ToString());
  public virtual void SetActive(bool value) => throw new NotImplementedException();
  public virtual void SetParent(Element parent, Element beforeChild = null) => throw new NotImplementedException();
  public virtual void Clear() => throw new NotImplementedException();
  public virtual Rect GetBoundingClientRect() => default;
}

[JSExport]
public class ObjectElement : Element
{
  protected ObjectElement(object obj) : base(obj) { }

  public delegate Task<object> Loader(string path);
  public static Loader LoadAsync { get; set; } = async (string path) =>
  {
    var request = Resources.LoadAsync(path);
    await request;
    return request.asset;
  };

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object)mObj);
  }
}

[JSExport]
public class GameObjectElement : ObjectElement
{
  protected GameObjectElement(object obj) : base(obj) { }

  private static GameObjectElement Wrap(GameObject obj) => obj ? new GameObjectElement(obj) : null;
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

  public static GameObjectElement Create(object kind)
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

[JSExport]
public class ComponentElement : ObjectElement
{
  public static IDictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

  static ComponentElement()
  {
    var types = PropertyBag.GetAllTypesWithAPropertyBag();
    foreach (var type in types.Where(type => type.IsSubclassOf(typeof(Component))))
      Types.Add(KeyValuePair.Create(type.Name.Uncapitalize(), type));
  }

  protected ComponentElement(Type type) : base(new Dictionary<string, object>() { { string.Empty, type } }) { }

  public static ComponentElement Create(object kind) => kind switch
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

  public override void SetProperty(string key, object value)
  {
    if (mObj is Dictionary<string, object> props)
      props[key] = value;
    else
      base.SetProperty(key, value);
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

    if (parent.mObj is GameObject && mObj is Dictionary<string, object> props)
    {
      var type = (Type)props[string.Empty];
      mObj = ((GameObject)parent.mObj).GetComponent(type) ?? ((GameObject)parent.mObj).AddComponent(type);
      foreach (var prop in props)
        if (prop.Key != string.Empty)
          base.SetProperty(prop.Key, prop.Value);
    }
    else
      base.SetParent(parent, beforeChild);
  }
}

public static class UnityNodeApi
{
  public static string Uncapitalize(this string s) => char.ToLower(s[0]) + s.Substring(1);

  public static IEnumerable<T> AsVector<T>(this string source) => source.Split(' ').Select(s => TypeConversion.Convert<string, T>(ref s));

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void Init()
  {
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<int>(); return new Vector2Int(p.ElementAt(0), p.ElementAt(1)); });
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<int>(); return new Vector3Int(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2)); });
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<float>(); return new Vector2(p.ElementAt(0), p.ElementAt(1)); });
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<float>(); return new Vector3(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2)); });
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<float>(); return new Vector4(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2), p.ElementAt(3)); });
    TypeConversion.Register((ref string s) => { if (s == default) return default; var p = s.AsVector<float>(); return new Quaternion(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2), p.ElementAt(3)); });
  }
}
