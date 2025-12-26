using System;
using System.Diagnostics;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;

[assembly: AlwaysLinkAssembly]
public static class UnityNodeApi
{
  [JSExport]
  public static object ActiveScene { get => SceneManager.GetActiveScene(); }

  [JSExport]
  public static object CreateObject(string type)
  {
    Trace.WriteLine($"CreateObject: {type}");
    if ("sphere".Equals(type))
    {
      return GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  [JSExport]
  public static void DeleteObject(object node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void AppendChildObject(object parent, object child)
  {
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
    throw new NotImplementedException();
  }

  [JSExport]
  public static void HideObject(object node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void UnhideObject(object node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void ClearObject(object container)
  {
    if (container is Scene && ((Scene)container) == SceneManager.GetActiveScene())
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    else
    {
      Trace.WriteLine($"Clearing container {container}");
      throw new NotImplementedException();
    }
  }
}
