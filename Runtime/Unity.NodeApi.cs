/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using Unity.Properties;

[assembly: AlwaysLinkAssembly]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.GameObject))]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.Transform))]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.Behaviour))]

[JSExport]
public class Instance : IDisposable
{
  internal object mObj;

  protected Instance(object obj) => mObj = obj;
  public virtual void Dispose() => (mObj as IDisposable)?.Dispose();

  public override bool Equals(object obj) => (obj is Instance instance) ? obj.Equals(instance.mObj) : base.Equals(obj);
  public override int GetHashCode() => mObj.GetHashCode();
  public override string ToString() => PropertiezDump.ToString(mObj);

  public virtual void SetProperty(string key, object value) => PropertyContainer.SetValue(mObj, key, value?.ToString());
  public virtual void SetActive(bool value) => throw new NotImplementedException();
  public virtual void SetParent(Instance parent, Instance beforeChild = null) => throw new NotImplementedException();
  public virtual void Clear() => throw new NotImplementedException();
}

[JSExport]
public class BaseObject : Instance
{
  protected BaseObject(object obj) : base(obj) { }

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object)mObj);
  }
}

[JSExport]
public class GameObject : BaseObject
{
  protected GameObject(object obj) : base(obj) { }

  private static GameObject Wrap(UnityEngine.GameObject obj) => obj ? new GameObject(obj) : null;
  public static GameObject Find(string name) => Wrap(UnityEngine.GameObject.Find(name));

  private static GameObject _null = new GameObject(null);
  private static GameObject Null
  {
    get
    {
      if (_null.mObj == null)
      {
        _null.mObj = new UnityEngine.GameObject();
        _null.SetActive(false);
      }
      return _null;
    }
  }

  public static GameObject Create(string type)
  {
    var obj = type switch
    {
      "object" => Wrap(new UnityEngine.GameObject()),
      "sphere" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere)),
      "capsule" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Capsule)),
      "cylinder" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cylinder)),
      "cube" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube)),
      "plane" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Plane)),
      "quad" => Wrap(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Quad)),
      _ => null,
    };
    obj?.SetParent(null);
    return obj;
  }

  public override void SetActive(bool value)
  {
    ((UnityEngine.GameObject)mObj).SetActive(value);
  }

  public override void SetParent(Instance parent, Instance beforeChild = null)
  {
    if (parent == null)
      parent = Null;

    if (parent.mObj is UnityEngine.GameObject)
    {
      ((UnityEngine.GameObject)mObj).transform.SetParent(((UnityEngine.GameObject)parent.mObj).transform, false);
      if (beforeChild is GameObject)
        ((UnityEngine.GameObject)mObj).transform.SetSiblingIndex(((UnityEngine.GameObject)beforeChild.mObj).transform.GetSiblingIndex());
    }
    else
      base.SetParent(parent, beforeChild);
  }

  public override void Clear()
  {
    foreach (UnityEngine.Transform child in ((UnityEngine.GameObject)mObj).transform)
      child.SetParent(((UnityEngine.GameObject)Null.mObj).transform, false);
  }
}

[JSExport]
public class Component : BaseObject
{
  protected Component(Type type) : base(new Dictionary<string, object>() { { string.Empty, type } }) { }

  public static Component Create(string type) => type switch
  {
    "transform" => new Component(typeof(UnityEngine.Transform)),
    _ => null,
  };

  public override void Dispose()
  {
    // Destroying the transform component is not allowed.
    if (mObj is UnityEngine.Transform transform)
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
    if (mObj is UnityEngine.Behaviour)
      ((UnityEngine.Behaviour)mObj).enabled = value;
    else
      base.SetActive(value);
  }

  public override void SetParent(Instance parent, Instance beforeChild = null)
  {
    if (parent == null)
    {
      Dispose();
      return;
    }

    if (parent.mObj is UnityEngine.GameObject && mObj is Dictionary<string, object> props)
    {
      var type = (Type)props[string.Empty];
      mObj = ((UnityEngine.GameObject)parent.mObj).GetComponent(type) ?? ((UnityEngine.GameObject)parent.mObj).AddComponent(type);
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
