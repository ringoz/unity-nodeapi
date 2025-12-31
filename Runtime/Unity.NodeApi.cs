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

  private static GameObject _Trash = new GameObject(null);
  private static GameObject Trash
  {
    get
    {
      if (_Trash.mObj == null)
      {
        _Trash.mObj = new UnityEngine.GameObject();
        _Trash.SetActive(false);
      }
      return _Trash;
    }
  }

  public static GameObject Create(string type)
  {
    var obj = type switch
    {
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
    Assert.IsNull(beforeChild);
    if (parent == null)
      parent = Trash;

    if (parent.mObj is UnityEngine.GameObject)
      ((UnityEngine.GameObject)mObj).transform.parent = ((UnityEngine.GameObject)parent.mObj).transform;
    else
      base.SetParent(parent, beforeChild);
  }

  public override void Clear()
  {
    foreach (Transform child in ((UnityEngine.GameObject)mObj).transform)
      child.parent = ((UnityEngine.GameObject)Trash.mObj).transform;
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
    Assert.IsNotNull(parent);
    Assert.IsNull(beforeChild);

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
  public static string RemoveParentheses(this string source) => source.Replace("(", "").Replace(")", "");
  public static IEnumerable<T> AsVector<T>(this string source) => source.RemoveParentheses().Split(',').Select(s => TypeConversion.Convert<string, T>(ref s));

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void Init()
  {
    TypeConversion.Register((ref string s) => { var p = s.AsVector<int>(); return new Vector2Int(p.ElementAt(0), p.ElementAt(1)); });
    TypeConversion.Register((ref string s) => { var p = s.AsVector<int>(); return new Vector3Int(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2)); });
    TypeConversion.Register((ref string s) => { var p = s.AsVector<float>(); return new Vector2(p.ElementAt(0), p.ElementAt(1)); });
    TypeConversion.Register((ref string s) => { var p = s.AsVector<float>(); return new Vector3(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2)); });
    TypeConversion.Register((ref string s) => { var p = s.AsVector<float>(); return new Vector4(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2), p.ElementAt(3)); });
    TypeConversion.Register((ref string s) => { var p = s.AsVector<float>(); return new Quaternion(p.ElementAt(0), p.ElementAt(1), p.ElementAt(2), p.ElementAt(3)); });
  }
}
