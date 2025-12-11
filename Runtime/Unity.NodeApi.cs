using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

public static class UnityNodeApi
{
  [JSExport]
  public static void Hello(string message)
  {
    Debug.Log(message);
  }
}
