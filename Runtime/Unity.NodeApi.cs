using System;
using System.Reflection;
using Microsoft.JavaScript.NodeApi;
using UnityEngine.Scripting;

using TType = System.String;
using TProps = System.Collections.Generic.IReadOnlyDictionary<string, object>;
using TContainer = System.Object;
using THostContext = System.Object;
using TInstance = System.Object;
using TTextInstance = System.Object;
using TPublicInstance = System.Object;

[assembly: AlwaysLinkAssembly]
public static class UnityNodeApi
{
  [JSExport]
  public static string RendererPackageName { get => Assembly.GetExecutingAssembly().GetName().Name; }

  [JSExport]
  public static string RendererVersion { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(); }

  [JSExport]
  public static bool SupportsMutation { get => true; }

  [JSExport]
  public static bool IsPrimaryRenderer { get => true; }

  [JSExport]
  public static TInstance CreateInstance(TType type, TProps props, TContainer rootContainer = null, THostContext hostContext = null)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static TTextInstance CreateTextInstance(string text, TContainer rootContainer = null, THostContext hostContext = null)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void AppendInitialChild(TInstance parent, TInstance child)
  {
    AppendChild(parent, child);
  }

  [JSExport]
  public static bool FinalizeInitialChildren(TInstance node, TType type, TProps props, TContainer rootContainer = null, THostContext hostContext = null)
  {
    return false;
  }

  [JSExport]
  public static bool ShouldSetTextContent(TType type, TProps props)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static THostContext GetRootHostContext(TContainer root = null)
  {
    return typeof(UnityNodeApi);
  }

  [JSExport]
  public static THostContext GetChildHostContext(THostContext parentHostContext, TType type, TContainer root = null)
  {
    return parentHostContext;
  }

  [JSExport]
  public static TPublicInstance GetPublicInstance(TInstance node)
  {
    return node;
  }

  [JSExport]
  public static void PrepareForCommit(TContainer containerInfo)
  {
  }

  [JSExport]
  public static void ResetAfterCommit(TContainer containerInfo)
  {
  }

  [JSExport]
  public static void PreparePortalMount(TContainer containerInfo)
  {
    throw new NotImplementedException();
  }

  private const int ContinuousEventPriority = 8;
  private const int DefaultEventPriority = 32;
  private const int DiscreteEventPriority = 2;
  private const int IdleEventPriority = 268435456;
  private const int NoEventPriority = 0;

  private static int CurrentUpdatePriority = NoEventPriority;

  [JSExport]
  public static void SetCurrentUpdatePriority(int newPriority)
  {
    CurrentUpdatePriority = newPriority;
  }
  
  [JSExport]
  public static int GetCurrentUpdatePriority()
  {
    return CurrentUpdatePriority;
  }

  [JSExport]
  public static int ResolveUpdatePriority()
  {
    if (CurrentUpdatePriority != NoEventPriority)
      return CurrentUpdatePriority;

    // TODO
    return DefaultEventPriority;
  }

  [JSExport]
  public static void TrackSchedulerEvent()
  {
  }
  
  [JSExport]
  public static string ResolveEventType()
  {
    return null;
  }

  [JSExport]
  public static double ResolveEventTimeStamp()
  {
    return -1.1;
  }

  [JSExport]
  public static void ResetFormInstance(object form)
  {
  }

  [JSExport]
  public static bool ShouldAttemptEagerTransition()
  {
    return false;
  }

  [JSExport]
  public static object GetInstanceFromNode(object node)
  {
    return null;
  }

  [JSExport]
  public static void DetachDeletedInstance(TInstance node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static bool MaySuspendCommit(TType type, TProps props)
  {
    return false;
  }

  [JSExport]
  public static bool PreloadInstance(TInstance node, TType type, TProps props)
  {
    return true; // indicate it's already loaded
  }

  [JSExport]
  public static object StartSuspendingCommit()
  {
    return null;
  }

  [JSExport]
  public static void SuspendInstance(object state, TInstance node, TType type, TProps props)
  {
  }

  [JSExport]
  public static object WaitForCommitToBeReady()
  {
    return null;
  }

  [JSExport]
  public static void AppendChild(TInstance parent, TInstance child)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void AppendChildToContainer(TContainer container, TInstance child)
  {
    AppendChild(container, child);
  }

  [JSExport]
  public static void InsertBefore(TInstance parent, TInstance child, TInstance beforeChild)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void InsertInContainerBefore(TContainer container, TInstance child, TInstance beforeChild)
  {
    InsertBefore(container, child, beforeChild);
  }

  [JSExport]
  public static void RemoveChild(TInstance parent, TInstance child)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void RemoveChildFromContainer(TContainer container, TInstance child)
  {
    RemoveChild(container, child);
  }

  [JSExport]
  public static void ResetTextContent(TInstance node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void CommitTextUpdate(TTextInstance node, string oldText, string newText)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void CommitMount(TInstance node, TType type, TProps props)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void CommitUpdate(TInstance node, TType type, TProps prevProps, TProps nextProps)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void HideInstance(TInstance node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void HideTextInstance(TTextInstance node)
  {
    HideInstance(node);
  }

  [JSExport]
  public static void UnhideInstance(TInstance node)
  {
    throw new NotImplementedException();
  }

  [JSExport]
  public static void UnhideTextInstance(TTextInstance node)
  {
    UnhideInstance(node);
  }

  [JSExport]
  public static void ClearContainer(TContainer container)
  {
    throw new NotImplementedException();
  }
}
