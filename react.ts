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

type Vector3 = number[];

interface Object {
  name: string;
}

interface GameObject extends Object {
}

interface Component extends Object {
}

interface Transform extends Component {
  localScale: Vector3;
  localPosition: Vector3;
}

export type Props<T> = React.PropsWithChildren<Partial<T>> & React.RefAttributes<T>;

export function /* @__PURE__ */ intrinsic<T>(type: string) {
  const render: React.FunctionComponent<Props<T>> = (props) => createElement(type, props);
  render.displayName = type;
  return render;
}

export const GameObject = intrinsic<GameObject>("GameObject");
export const Capsule = intrinsic<GameObject>("Capsule");
export const Cube = intrinsic<GameObject>("Cube");
export const Cylinder = intrinsic<GameObject>("Cylinder");
export const Plane = intrinsic<GameObject>("Plane");
export const Quad = intrinsic<GameObject>("Quad");
export const Sphere = intrinsic<GameObject>("Sphere");
export const Transform = intrinsic<Transform>("Transform");

export function /* @__PURE__ */ asset<T = GameObject>(path: string) {
  return lazy(async () => {
    const $$$ = await Element.loadAssetAsync(path);
    const render: React.FunctionComponent<Props<T>> = (props) => createElement("", { $$$, ...props });
    render.displayName = path;
    return { default: render };
  });
}
