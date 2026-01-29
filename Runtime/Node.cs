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
using System.Collections.Generic;

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

  public static JSValue ToJSArray(this IEnumerable<JSValue> values)
  {
    var array = JSValue.CreateArray(); int i = 0;
    foreach (var value in values)
      array.SetElement(i++, value);
    return array;
  }

  public static IEnumerable<float> AsEnumerable(this Vector2 value)
  {
    yield return value.x;
    yield return value.y;
  }

  public static IEnumerable<int> AsEnumerable(this Vector2Int value)
  {
    yield return value.x;
    yield return value.y;
  }

  public static IEnumerable<float> AsEnumerable(this Vector3 value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.z;
  }

  public static IEnumerable<int> AsEnumerable(this Vector3Int value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.z;
  }

  public static IEnumerable<float> AsEnumerable(this Vector4 value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.z;
    yield return value.w;
  }

  public static IEnumerable<float> AsEnumerable(this Quaternion value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.z;
    yield return value.w;
  }

  public static IEnumerable<float> AsEnumerable(this Matrix4x4 value)
  {
    yield return value.m00;
    yield return value.m01;
    yield return value.m02;
    yield return value.m03;

    yield return value.m10;
    yield return value.m11;
    yield return value.m12;
    yield return value.m13;

    yield return value.m20;
    yield return value.m21;
    yield return value.m22;
    yield return value.m23;

    yield return value.m30;
    yield return value.m31;
    yield return value.m32;
    yield return value.m33;
  }

  public static IEnumerable<float> AsEnumerable(this Color value)
  {
    yield return value.r;
    yield return value.g;
    yield return value.b;
    yield return value.a;
  }

  public static IEnumerable<float> AsEnumerable(this Rect value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.width;
    yield return value.height;
  }

  public static IEnumerable<int> AsEnumerable(this RectInt value)
  {
    yield return value.x;
    yield return value.y;
    yield return value.width;
    yield return value.height;
  }

  public static IEnumerable<float> AsEnumerable(this Bounds value)
  {
    yield return value.center.x;
    yield return value.center.y;
    yield return value.center.z;

    yield return value.size.x;
    yield return value.size.y;
    yield return value.size.z;
  }

  public static IEnumerable<int> AsEnumerable(this BoundsInt value)
  {
    yield return value.position.x;
    yield return value.position.y;
    yield return value.position.z;

    yield return value.size.x;
    yield return value.size.y;
    yield return value.size.z;
  }

  public static Vector2 ToVector2(this IEnumerable<float> value)
  {
    var result = new Vector2();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
    }
    return result;
  }

  public static Vector2Int ToVector2Int(this IEnumerable<int> value)
  {
    var result = new Vector2Int();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
    }
    return result;
  }

  public static Vector3 ToVector3(this IEnumerable<float> value)
  {
    var result = new Vector3();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.z = values.Current;
    }
    return result;
  }

  public static Vector3Int ToVector3Int(this IEnumerable<int> value)
  {
    var result = new Vector3Int();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.z = values.Current;
    }
    return result;
  }

  public static Vector4 ToVector4(this IEnumerable<float> value)
  {
    var result = new Vector4();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.z = values.Current;
      values.MoveNext(); result.w = values.Current;
    }
    return result;
  }

  public static Quaternion ToQuaternion(this IEnumerable<float> value)
  {
    var result = new Quaternion();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.z = values.Current;
      values.MoveNext(); result.w = values.Current;
    }
    return result;
  }

  public static Matrix4x4 ToMatrix4x4(this IEnumerable<float> value)
  {
    var result = new Matrix4x4();
    using (var values = value.GetEnumerator())
    {
      for (int i = 0; i < 16; i++)
      {
        values.MoveNext(); result[i] = values.Current;
      }
    }
    return result;
  }

  public static Color ToColor(this IEnumerable<float> value)
  {
    var result = new Color();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.r = values.Current;
      values.MoveNext(); result.g = values.Current;
      values.MoveNext(); result.b = values.Current;
      values.MoveNext(); result.a = values.Current;
    }
    return result;
  }

  public static Rect ToRect(this IEnumerable<float> value)
  {
    var result = new Rect();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.width = values.Current;
      values.MoveNext(); result.height = values.Current;
    }
    return result;
  }

  public static RectInt ToRectInt(this IEnumerable<int> value)
  {
    var result = new RectInt();
    using (var values = value.GetEnumerator())
    {
      values.MoveNext(); result.x = values.Current;
      values.MoveNext(); result.y = values.Current;
      values.MoveNext(); result.width = values.Current;
      values.MoveNext(); result.height = values.Current;
    }
    return result;
  }

  public static Bounds ToBounds(this IEnumerable<float> value)
  {
    var result = new Bounds();
    using (var values = value.GetEnumerator())
    {
      var tmp = new Vector3();
      values.MoveNext(); tmp.x = values.Current;
      values.MoveNext(); tmp.y = values.Current;
      values.MoveNext(); tmp.z = values.Current;
      result.center = tmp;
      values.MoveNext(); tmp.x = values.Current;
      values.MoveNext(); tmp.y = values.Current;
      values.MoveNext(); tmp.z = values.Current;
      result.size = tmp;
    }
    return result;
  }

  public static BoundsInt ToBoundsInt(this IEnumerable<int> value)
  {
    var result = new BoundsInt();
    using (var values = value.GetEnumerator())
    {
      var tmp = new Vector3Int();
      values.MoveNext(); tmp.x = values.Current;
      values.MoveNext(); tmp.y = values.Current;
      values.MoveNext(); tmp.z = values.Current;
      result.position = tmp;
      values.MoveNext(); tmp.x = values.Current;
      values.MoveNext(); tmp.y = values.Current;
      values.MoveNext(); tmp.z = values.Current;
      result.size = tmp;
    }
    return result;
  }
}

[JSExport]
public class Node : IDisposable
{
  protected static TypeConverter<T?, JSValue> JS<T>(JSValue.From<T> convert)
  {
    return (ref T? v) => v != null ? convert(v) : default;
  }

  protected static TypeConverter<JSValue, T?> JS<T>(JSValue.To<T> convert)
  {
    return (ref JSValue v) => v.IsNullOrUndefined() ? default : convert(v);
  }

  static Node()
  {
    TypeConversion.Register(JS((char v) => (JSValue)v));
    TypeConversion.Register(JS((bool v) => (JSValue)v));
    TypeConversion.Register(JS((sbyte v) => (JSValue)v));
    TypeConversion.Register(JS((short v) => (JSValue)v));
    TypeConversion.Register(JS((int v) => (JSValue)v));
    TypeConversion.Register(JS((long v) => (JSValue)v));
    TypeConversion.Register(JS((byte v) => (JSValue)v));
    TypeConversion.Register(JS((ushort v) => (JSValue)v));
    TypeConversion.Register(JS((uint v) => (JSValue)v));
    TypeConversion.Register(JS((ulong v) => (JSValue)v));
    TypeConversion.Register(JS((float v) => (JSValue)v));
    TypeConversion.Register(JS((double v) => (JSValue)v));
    TypeConversion.Register(JS((string v) => (JSValue)v));
    TypeConversion.Register(JS((object v) => (JSValue)v));
    TypeConversion.Register(JS((int[] v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((float[] v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((string[] v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((object[] v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((List<int> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((List<float> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((List<string> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((List<object> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((IEnumerable<int> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((IEnumerable<float> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((IEnumerable<string> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((IEnumerable<object> v) => v.Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Action v) => new JSFunction(v)));
    TypeConversion.Register(JS((Action<Event> v) => new JSFunction((JSValue e) => v((Event)e.Unwrap("Event")))));
    TypeConversion.Register(JS((PropertyPath v) => (JSValue)v.ToString()));
    TypeConversion.Register(JS((Vector2 v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Vector2Int v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Vector3 v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Vector3Int v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Vector4 v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Quaternion v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Matrix4x4 v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Color v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Rect v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((RectInt v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((Bounds v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));
    TypeConversion.Register(JS((BoundsInt v) => v.AsEnumerable().Select(v => (JSValue)v).ToJSArray()));

    TypeConversion.Register(JS((JSValue v) => (char)v));
    TypeConversion.Register(JS((JSValue v) => (bool)v));
    TypeConversion.Register(JS((JSValue v) => (sbyte)v));
    TypeConversion.Register(JS((JSValue v) => (short)v));
    TypeConversion.Register(JS((JSValue v) => (int)v));
    TypeConversion.Register(JS((JSValue v) => (long)v));
    TypeConversion.Register(JS((JSValue v) => (byte)v));
    TypeConversion.Register(JS((JSValue v) => (ushort)v));
    TypeConversion.Register(JS((JSValue v) => (uint)v));
    TypeConversion.Register(JS((JSValue v) => (ulong)v));
    TypeConversion.Register(JS((JSValue v) => (float)v));
    TypeConversion.Register(JS((JSValue v) => (double)v));
    TypeConversion.Register(JS((JSValue v) => (string)v));
    TypeConversion.Register(JS((JSValue v) => (object)v));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToArray()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToArray()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (string)v).ToArray()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (object)v).ToArray()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToList()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToList()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (string)v).ToList()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (object)v).ToList()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v)));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v)));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (string)v)));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (object)v)));
    TypeConversion.Register(JS((JSValue v) => v.ToAction()));
    TypeConversion.Register(JS((JSValue v) => v.ToAction<Event>()));
    TypeConversion.Register(JS((JSValue v) => new PropertyPath((string)v)));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToVector2()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToVector2Int()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToVector3()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToVector3Int()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToVector4()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToQuaternion()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToMatrix4x4()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToColor()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToRect()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToRectInt()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (float)v).ToBounds()));
    TypeConversion.Register(JS((JSValue v) => v.Items.Select(v => (int)v).ToBoundsInt()));
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
  public virtual JSValue GetProp(string path) => Propertiez.GetJSValue(mPtr, PropPath(path));
  public virtual void SetProps(in JSValue props) { foreach (var item in (JSObject)props) Propertiez.SetJSValue(mPtr, PropPath(item.Key), item.Value); }
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
