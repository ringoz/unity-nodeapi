/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

using System.Linq;
using UnityEngine;

class GameObjectNode : Node
{
  protected GameObjectNode(object ptr) : base(ptr) { }

  public static Node Wrap(GameObject obj) => obj != null ? Wrappers.GetValue(obj, obj => new GameObjectNode(obj)) : null;
  public static Node Find(string name) => Wrap(GameObject.Find(name));

  private static GameObjectNode _null = new GameObjectNode(null);
  private static GameObjectNode Null
  {
    get
    {
      if (_null.mPtr == null)
      {
        _null.mPtr = new GameObject();
        _null.SetActive(false);
      }
      return _null;
    }
  }

  public static new Node Create(object kind)
  {
    if (kind is GameObject prefab)
      return Wrap(UnityEngine.Object.Instantiate(prefab, ((GameObject)Null.mPtr).transform, false));
    return null;
  }

  public override void Dispose()
  {
    UnityEngine.Object.Destroy((GameObject)mPtr);
  }

  public override void SetActive(bool value)
  {
    ((GameObject)mPtr).SetActive(value);
  }

  public override void SetParent(Node parent, Node beforeChild = null)
  {
    var parentGameObject = (GameObject)(parent ?? Null).mPtr;
    ((GameObject)mPtr).transform.SetParent(parentGameObject.transform, false);
    if (beforeChild?.mPtr is GameObject beforeChildGameObject)
      ((GameObject)mPtr).transform.SetSiblingIndex(beforeChildGameObject.transform.GetSiblingIndex());
  }

  public override void Clear()
  {
    foreach (Transform child in ((GameObject)mPtr).transform)
      child.SetParent(((GameObject)Null.mPtr).transform, false);
  }

  public override DOMRect GetBoundingClientRect()
  {
    var renderer = ((GameObject)mPtr).GetComponent<Renderer>();
    if (renderer == null)
      return base.GetBoundingClientRect();

    var c = renderer.bounds.center;
    var e = renderer.bounds.extents;
    var corners = new[]
    {
      new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
    }.Select(corner => Camera.main.WorldToScreenPoint(corner));

    var maxX = corners.Max(corner => corner.x) * 96 / Screen.dpi;
    var minX = corners.Min(corner => corner.x) * 96 / Screen.dpi;
    var maxY = corners.Max(corner => Screen.height - corner.y) * 96 / Screen.dpi;
    var minY = corners.Min(corner => Screen.height - corner.y) * 96 / Screen.dpi;
    return new DOMRect() { x = minX, y = minY, width = maxX - minX, height = maxY - minY };
  }
}
