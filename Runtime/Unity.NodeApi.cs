/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Text;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using Unity.Properties;

[assembly: AlwaysLinkAssembly]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.GameObject))]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.Transform))]
[assembly: GeneratePropertyBagsForType(typeof(UnityEngine.Behaviour))]

[JSExport]
public class Instance : IDisposable
{
  internal object m_obj;
  internal object obj { get => m_obj; init => m_obj = value; }

  protected Instance() { }

  public virtual void Dispose()
  {
    if (obj is IDisposable)
      ((IDisposable)obj).Dispose();
  }

  public override string ToString()
  {
    var sb = new StringBuilder();
    PropertyContainer.Accept(new PropertiezDump((l) => sb.AppendLine(l)), this);
    return sb.ToString();
  }

  public void SetProperty(string property, object value)
  {
    PropertyContainer.SetValue(obj, property, value);
  }

  public virtual void SetActive(bool value)
  {
    throw new NotImplementedException();
  }

  public virtual void SetParent(Instance parent, Instance beforeChild = null)
  {
    throw new NotImplementedException();
  }

  public virtual void Clear()
  {
    throw new NotImplementedException();
  }
}

[JSExport]
public class Scene : Instance
{
  protected Scene() { }

  public static Instance Active { get; } = new Scene() { obj = SceneManager.GetActiveScene() };

  public override void Clear()
  {
    if ((UnityEngine.SceneManagement.Scene)obj == SceneManager.GetActiveScene())
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}

[JSExport]
public class BaseObject : Instance
{
  protected BaseObject() { }
}

[JSExport]
public class GameObject : BaseObject
{
  protected GameObject() { }

  public static Instance Create(string type) => type switch
  {
    "sphere" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere) },
    "capsule" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Capsule) },
    "cylinder" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cylinder) },
    "cube" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube) },
    "plane" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Plane) },
    "quad" => new GameObject() { obj = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Quad) },
    _ => null,
  };

  public override void SetActive(bool value)
  {
    ((UnityEngine.GameObject)obj).SetActive(value);
  }

  public override void SetParent(Instance parent, Instance beforeChild = null)
  {
    if (parent.obj is UnityEngine.GameObject)
      ((UnityEngine.GameObject)obj).transform.parent = ((UnityEngine.GameObject)parent.obj).transform;
    else if (parent.obj is UnityEngine.SceneManagement.Scene)
      SceneManager.MoveGameObjectToScene((UnityEngine.GameObject)obj, (UnityEngine.SceneManagement.Scene)parent.obj);
    else
      base.SetParent(parent, beforeChild);
  }
}

[JSExport]
public class Component : BaseObject
{
  protected Component() { }

  public static Instance Create(string type) => type switch
  {
    _ => null,
  };

  public override void SetActive(bool value)
  {
    if (obj is UnityEngine.Behaviour)
      ((UnityEngine.Behaviour)obj).enabled = value;
    else
      base.SetActive(value);
  }
}

public static class UnityNodeApi
{
}
