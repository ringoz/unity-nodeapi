/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import FiberConfig from './index.js';

const reconciler = Reconciler(FiberConfig);
reconciler.injectIntoDevTools();
reconciler.flushSync = reconciler.flushSyncFromReconciler;

export function createRoot(view) {
  const isStrictMode = process.env.NODE_ENV !== 'production';
  const concurrentUpdatesByDefaultOverride = false;
  const identifierPrefix = '';
  const onUncaughtError = reconciler.defaultOnUncaughtError;
  const onCaughtError = reconciler.defaultOnCaughtError;
  const onRecoverableError = reconciler.defaultOnRecoverableError;
  const onDefaultTransitionIndicator = () => { };
  const transitionCallbacks = null;
  const root = reconciler.createContainer(
    view,
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
    render: (component) => new Promise((resolve, reject) => {
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
