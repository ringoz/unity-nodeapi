/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/
#nullable enable

using System;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[JSExport]
public abstract class Event : IDisposable
{
  public override string ToString() => $"[{GetType().Name}] {Type}";
  public abstract void Dispose();
  public virtual string Type => GetType().Name;
  public virtual long Timestamp => (long)(Time.unscaledTime * 1000.0f);
  public abstract Node Target { get; }
  public virtual JSValue? Value => default;

  public static void InjectMouseClick(float x, float y)
  {
    var screenPos = new Vector2(x, y) * Screen.dpi / 96;
    screenPos.y = Screen.height - screenPos.y;

    var mouse = Mouse.current;
    InputSystem.QueueStateEvent(mouse, new MouseState { position = screenPos });
    InputSystem.QueueStateEvent(mouse, new MouseState { position = screenPos }.WithButton(MouseButton.Left));
    InputSystem.QueueStateEvent(mouse, new MouseState { position = screenPos });
    InputSystem.Update();
  }
}
