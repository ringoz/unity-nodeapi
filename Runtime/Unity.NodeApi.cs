/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Diagnostics;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using Unity.Properties;

[assembly: AlwaysLinkAssembly]
[assembly: GeneratePropertyBagsForType(typeof(GameObject))]
[assembly: GeneratePropertyBagsForType(typeof(Behaviour))]

public static class UnityNodeApi
{
  [JSExport]
  public static object ActiveScene => SceneManager.GetActiveScene();

  [JSExport]
  public static object CreateObject(string type)
  {
    Trace.WriteLine($"CreateObject: {type}");
    switch (type)
    {
      case "sphere":
        return GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //case "transform":
        //return new Transform();
        //case "meshRenderer":
        //return new MeshRenderer();
    }

    throw new NotImplementedException();
  }

  [JSExport]
  public static void DeleteObject(object node)
  {
    if (node is UnityEngine.Object)
      UnityEngine.Object.Destroy((UnityEngine.Object)node);
    else
      throw new NotImplementedException();
  }

  [JSExport]
  public static void AppendChildObject(object parent, object child)
  {
    if (parent is GameObject && child is GameObject)
      ((GameObject)child).transform.parent = ((GameObject)parent).transform;
    else if (parent is Scene && child is GameObject)
      SceneManager.MoveGameObjectToScene((GameObject)child, (Scene)parent);
    else
      throw new NotImplementedException();
  }

  [JSExport]
  public static void InsertBeforeObject(object parent, object child, object beforeChild)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void RemoveChildObject(object parent, object child)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void SetObjectProperty(object node, string property, object value)
  {
    PropertyContainer.SetValue(node, property, value);
  }

  [JSExport]
  public static void DumpObjectProperties(object node)
  {
    PropertyContainer.Accept(new PropertiezDump(), node);
  }

  [JSExport]
  public static void HideObject(object node)
  {
    if (node is GameObject)
      ((GameObject)node).SetActive(false);
    else if (node is Behaviour)
      ((Behaviour)node).enabled = false;
    else
      throw new NotImplementedException();
  }

  [JSExport]
  public static void UnhideObject(object node)
  {
    if (node is GameObject)
      ((GameObject)node).SetActive(true);
    else if (node is Behaviour)
      ((Behaviour)node).enabled = true;
    else
      throw new NotImplementedException();
  }

  [JSExport]
  public static void ClearObject(object container)
  {
    if (container is Scene && ((Scene)container) == SceneManager.GetActiveScene())
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    else
      throw new NotImplementedException();
  }
}
