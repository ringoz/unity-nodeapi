/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.JavaScript.NodeApi;
using UnityEngine;
using Unity.Properties;

#nullable enable

[JSExport]
public struct DOMRect
{
  public float x { get; init; }
  public float y { get; init; }
  public float width { get; init; }
  public float height { get; init; }
  public float left => x + 0.5f;
  public float top => y + 0.5f;
  public float right => left + width;
  public float bottom => top + height;
}

[JSExport]
public abstract class Event : IDisposable
{
  public override string ToString() => $"[{GetType().Name}] {Type}";
  public abstract void Dispose();
  public virtual string Type => GetType().Name;
  public virtual long Timestamp => (long)(Time.unscaledTime * 1000.0f);
  public abstract Node Target { get; }
  public virtual JSValue? Value => default;
}

[JSExport]
public delegate Task<object> Loader(string path);

static class JSValueExtensions
{
  public static Action ToAction(this JSValue value)
  {
    var reference = new JSReference(value);
    return new Action(() =>
    {
      using var scope = new JSValueScope(JSValueScopeType.Callback);
      reference.GetValue().Call();
    });
  }

  public static Action<A> ToAction<A>(this JSValue value) where A : class
  {
    var reference = new JSReference(value);
    return new Action<A>((A a) =>
    {
      using var scope = new JSValueScope(JSValueScopeType.Callback);
      reference.GetValue().Call(JSValue.Undefined, scope.RuntimeContext.GetOrCreateObjectWrapper(a));
    });
  }

  public static float[] ToArray(this Vector2 value) => new float[] { value.x, value.y };
  public static float[] ToArray(this Vector3 value) => new float[] { value.x, value.y, value.z };
  public static float[] ToArray(this Vector4 value) => new float[] { value.x, value.y, value.z, value.w };
  public static float[] ToArray(this Rect value) => new float[] { value.x, value.y, value.width, value.height };
}

[JSExport]
public class Node : IDisposable
{
  static Node()
  {
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (char)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (bool)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (sbyte)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (short)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (int)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (long)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (byte)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (ushort)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (uint)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (ulong)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (float)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (double)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (string)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : (object)v);
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (int)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (float)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (string)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (object)v).ToArray());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (int)v).ToList());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (float)v).ToList());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (string)v).ToList());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (object)v).ToList());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (int)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (float)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (string)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.Items.Select(v => (object)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.ToAction());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : v.ToAction<Event>());
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new PropertyPath((string)v));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2((float)v[0], (float)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector2Int((int)v[0], (int)v[1]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3((float)v[0], (float)v[1], (float)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector3Int((int)v[0], (int)v[1], (int)v[2]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Quaternion((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Matrix4x4(new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]), new Vector4((float)v[4], (float)v[5], (float)v[6], (float)v[7]), new Vector4((float)v[8], (float)v[9], (float)v[10], (float)v[11]), new Vector4((float)v[12], (float)v[13], (float)v[14], (float)v[15])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Color((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Rect((float)v[0], (float)v[1], (float)v[2], (float)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new RectInt((int)v[0], (int)v[1], (int)v[2], (int)v[3]));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Bounds(new Vector3((float)v[0], (float)v[1], (float)v[2]), new Vector3((float)v[3], (float)v[4], (float)v[5])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new BoundsInt(new Vector3Int((int)v[0], (int)v[1], (int)v[2]), new Vector3Int((int)v[3], (int)v[4], (int)v[5])));
    TypeConversion.Register((ref JSValue v) => v.IsUndefined() ? default : new Version((int)v[0], (int)v[1], (int)v[2], (int)v[3]));
  }

  protected static readonly ConditionalWeakTable<object, Node> Wrappers = new();

  internal object mPtr;
  public virtual object Ptr => mPtr;

  protected Node(object ptr) => mPtr = ptr;
  public virtual void Dispose() => (mPtr as IDisposable)?.Dispose();

  public override bool Equals(object? other) => (GetType() == other?.GetType()) ? Equals(mPtr, ((Node)other).mPtr) : base.Equals(other);
  public override int GetHashCode() => mPtr?.GetHashCode() ?? 0;
  public override string ToString() => $"[{GetType().Name}] {mPtr}";

  private static PropertyPath PropPath(in JSValue key) => new PropertyPath(((string)key).Replace('-', '.'));
  private void SetProp(in PropertyPath key, in JSValue val)
  {
    switch (val.TypeOf())
    {
      case JSValueType.String:
        if (!Propertiez.TrySetValue(mPtr, key, (string)val, out _))
          Propertiez.SetValue(mPtr, key, val);
        break;

      case JSValueType.External:
        var obj = val.TryGetValueExternal();
        if (obj is UnityEngine.Object ueo)
          Propertiez.SetValue(mPtr, key, ueo);
        else
          Propertiez.SetValue(mPtr, key, obj);
        break;

      default:
        var part = key[key.Length - 1];
        if (part.IsName && (part.Name.EndsWith("Flags") || part.Name.EndsWith("Hints")) && val.IsArray())
          Propertiez.SetValue(mPtr, key, string.Join(',', ((JSArray)val).Select(v => (string)v)));
        else
          Propertiez.SetValue(mPtr, key, val);
        break;
    }
  }

  public virtual void SetProps(in JSValue props) { foreach (var item in (JSObject)props) SetProp(PropPath(item.Key), item.Value); }
  public virtual void SetActive(bool value) => throw new NotImplementedException();
  public virtual void SetParent(Node? parent, Node? beforeChild = null) => throw new NotImplementedException();
  public virtual void Clear() => throw new NotImplementedException();
  public virtual void Invoke(string methodName, in JSValue value = default) => mPtr.GetType().GetMethod(methodName)?.Invoke(mPtr, value != default ? new object[] { value } : Array.Empty<object>());
  public virtual DOMRect? GetBoundingClientRect() => null;

  private static Event? mEvent = null;
  public static Event? Event => mEvent;

  internal static void InvokeHandler<TEvent>(Action<TEvent>? handler, TEvent e) where TEvent : Event
  {
    var was = mEvent; mEvent = e;
    try { handler?.Invoke(e); }
    finally { mEvent = was; e.Dispose(); }
  }

  private static Node? CreateImpl(object kind) => AttributeOverridesNode.Create(kind) ?? VisualElementNode.Create(kind) ?? ComponentNode.Create(kind) ?? GameObjectNode.Create(kind);
  public static Node? Create(object kind) => CreateImpl(kind is string path ? Resources.Load(path) ?? kind : kind);
  public static Node? Search(object name, Node? scope = null) => scope is VisualElementNode root ? VisualElementNode.Find(name, root) : scope is GameObjectNode gobj ? ComponentNode.Find(name, gobj) : GameObjectNode.Find((string)name);

  public static Loader LoadAssetAsync { get; set; } = async (string path) =>
  {
    var request = Resources.LoadAsync(path);
    await request;
    return request.asset;
  };
}
