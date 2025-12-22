const { Module } = await createUnityInstance(document.querySelector("#unity-canvas"), {
  arguments: [],
  dataUrl: "Build.web.data",
  frameworkUrl: "Build.web.framework.js",
  codeUrl: "Build.web.wasm",
  streamingAssetsUrl: "StreamingAssets",
  productName: document.title,
  companyName: document.querySelector("meta[name='author']").content,
  productVersion: document.querySelector("meta[name='version']").content,
  // matchWebGLToCanvasSize: false, // Uncomment this to separately control WebGL canvas render size and DOM element size.
  // devicePixelRatio: 1, // Uncomment this to override low DPI rendering on high DPI displays.
});

import { getDefaultContext } from './Runtime/include/emnapi.mjs';
const exports = Module.emnapiInit({ context: getDefaultContext() });

exports.noTimeout = -1;
exports.scheduleTimeout = setTimeout;
exports.cancelTimeout = clearTimeout;
exports.supportsMicrotasks = true;
exports.scheduleMicrotask = queueMicrotask;

export default exports;
