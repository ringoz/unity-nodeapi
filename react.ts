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

export function /* @__PURE__ */ prefab(path: string) {
  return lazy(async () => {
    const $$$ = await Element.loadAssetAsync(path);
    const fun = (props: any) => createElement("prefab", { $$$, ...props });
    fun.displayName = path;
    return { default: fun };
  });
}
