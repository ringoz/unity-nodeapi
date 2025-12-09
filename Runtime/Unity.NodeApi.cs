using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

public static class UnityNodeApi
{
  public static object garbagez;

  [JSExport]
  public static void Hello()
  {
    Debug.Log("Hello world!");
    garbagez = new byte[1024];
  }
}
