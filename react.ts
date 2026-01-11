/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import { createElement, lazy } from 'react';
import Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import { Element } from '.';
import * as FiberConfig from './reconciler.ts';

const reconciler = Reconciler(FiberConfig);
reconciler.injectIntoDevTools(undefined as any);
// @ts-ignore - reconciler types are not maintained
reconciler.flushSync = reconciler.flushSyncFromReconciler;

export function createRoot(parent: Element) {
  const isStrictMode = process.env.NODE_ENV !== 'production';
  const concurrentUpdatesByDefaultOverride = false;
  const identifierPrefix = '';
  const onUncaughtError = (reconciler as any).defaultOnUncaughtError;
  const onCaughtError = (reconciler as any).defaultOnCaughtError;
  const onRecoverableError = (reconciler as any).defaultOnRecoverableError;
  const onDefaultTransitionIndicator = () => { };
  const transitionCallbacks = null;
  const root = reconciler.createContainer(
    parent,
    Constants.ConcurrentRoot,
    null,
    isStrictMode,
    concurrentUpdatesByDefaultOverride,
    identifierPrefix,
    onUncaughtError,
    onCaughtError,
    onRecoverableError,
    onDefaultTransitionIndicator,
    transitionCallbacks
  );
  return {
    render: (component: React.ReactNode) => new Promise<void>((resolve, reject) => {
      try {
        reconciler.updateContainer(component, root, null, resolve);
      } catch (e) {
        reject(e);
      }
    }),
    unmount: () => {
      reconciler.flushSync(() => reconciler.updateContainer(null, root, null));
      return Promise.resolve();
    }
  };
}

type IfEquals<X, Y, A = X, B = never> =
  (<T>() => T extends X ? 1 : 2) extends
  (<T>() => T extends Y ? 1 : 2) ? A : B;

type WritableKeys<T> = {
  [P in keyof T]-?: IfEquals<
    { [Q in P]: T[P] },
    { -readonly [Q in P]: T[P] },
    P
  >
}[keyof T];

type PickWritable<T> = Pick<T, WritableKeys<T>>;

export type Props<T> = React.PropsWithChildren<Partial<PickWritable<T>>> & React.RefAttributes<T>;

export function /* @__PURE__ */ intrinsic<T>(type: string) {
  const render: React.FunctionComponent<Props<T>> = (props) => createElement(type, props);
  render.displayName = type;
  return render;
}

export function /* @__PURE__ */ asset<T = GameObject>(path: string) {
  return lazy(async () => {
    const $$$ = await Element.loadAssetAsync(path);
    const render: React.FunctionComponent<Props<T>> = (props) => createElement("", { $$$, ...props });
    render.displayName = path;
    return { default: render };
  });
}

export type Ptr<T> = object;
export type Boolean = boolean;
export type Int16 = number;
export type UInt16 = number;
export type Int32 = number;
export type UInt32 = number;
export type Int64 = number;
export type UInt64 = number;
export type Single = number;
export type Double = number;
export type String = string;
export type Vector2 = [x: number, y: number];
export type Vector2Int = Vector2;
export type Vector3 = [x: number, y: number, z: number];
export type Vector3Int = Vector3;
export type Vector4 = [x: number, y: number, z: number, w: number];
export type Quaternion = Vector4;
export type Matrix4x4 = [number, number, number, number, number, number, number, number, number, number, number, number, number, number, number, number];
export type Color = [r: number, g: number, b: number, a: number];
export type Rect = [x: number, y: number, w: number, h: number];
export type RectInt = Rect;
export type Bounds = [x: number, y: number, z: number, sx: number, sy: number, sz: number];
export type BoundsInt = Bounds;
export type Version = [major: number, minor: number, build: number, revision: number];

export const Capsule = intrinsic<GameObject>("Capsule");
export const Cube = intrinsic<GameObject>("Cube");
export const Cylinder = intrinsic<GameObject>("Cylinder");
export const Plane = intrinsic<GameObject>("Plane");
export const Quad = intrinsic<GameObject>("Quad");
export const Sphere = intrinsic<GameObject>("Sphere");

//#region generated

export interface ObjectBase {
  name: String;
//hideFlags: HideFlags;
}

export interface GameObject extends ObjectBase {
  readonly transform: Ptr<Transform>;
//readonly transformHandle: TransformHandle;
  layer: Int32;
  readonly activeSelf: Boolean;
  readonly activeInHierarchy: Boolean;
  isStatic: Boolean;
  tag: String;
//readonly scene: Scene;
  readonly sceneCullingMask: UInt64;
  readonly gameObject: Ptr<GameObject>;
}
export const GameObject = intrinsic<GameObject>("GameObject");

export interface Component extends ObjectBase {
  readonly transform: Ptr<Transform>;
//readonly transformHandle: TransformHandle;
  readonly gameObject: Ptr<GameObject>;
  tag: String;
}
export const Component = intrinsic<Component>("Component");

export interface Transform extends Component {
  position: Vector3;
  localPosition: Vector3;
  eulerAngles: Vector3;
  localEulerAngles: Vector3;
  right: Vector3;
  up: Vector3;
  forward: Vector3;
  rotation: Quaternion;
  localRotation: Quaternion;
  localScale: Vector3;
  parent: Ptr<Transform>;
  readonly worldToLocalMatrix: Matrix4x4;
  readonly localToWorldMatrix: Matrix4x4;
  readonly root: Ptr<Transform>;
  readonly childCount: Int32;
  readonly lossyScale: Vector3;
  hasChanged: Boolean;
  hierarchyCapacity: Int32;
  readonly hierarchyCount: Int32;
}
export const Transform = intrinsic<Transform>("Transform");

//#endregion generated
