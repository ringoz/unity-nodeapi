import { name as productName, author as companyName, version as productVersion } from '../../../package.json';
const { Module } = await createUnityInstance(document.querySelector("#unity-canvas"), {
  arguments: [],
  dataUrl: "Build/Build.web.data",
  frameworkUrl: "Build/Build.web.framework.js",
  codeUrl: "Build/Build.web.wasm",
  streamingAssetsUrl: "StreamingAssets",
  productName,
  companyName,
  productVersion,
});

Module.errorHandler = () => true;
window.onerror = (message, file, line, column, error) => {
  const stack = error?.stack;
  const ErrorOverlay = customElements?.get('vite-error-overlay');
  if (ErrorOverlay) {
    const loc = line && column ? { file, line, column } : undefined;
    document.body.appendChild(new ErrorOverlay({ message, stack, loc }))
  } else {
    window.alert(message + '\n' + stack);
  }
};

import { getDefaultContext } from './Runtime/include/emnapi.mjs';
const context = getDefaultContext();
const exports = Module.emnapiInit({ context });
export default exports;

const raf = window.requestAnimationFrame;
window.requestAnimationFrame = (callback) => raf((time) => {
  const scope = context.openScope();
  try {
    callback(time);
  } finally {
    context.closeScope(scope);
  }
});
