const { Module } = await createUnityInstance(document.querySelector("#unity-canvas"), {
  arguments: [],
  dataUrl: "Build/Build.web.data",
  frameworkUrl: "Build/Build.web.framework.js",
  codeUrl: "Build/Build.web.wasm",
  streamingAssetsUrl: "StreamingAssets",
  productName: document.title,
  companyName: document.querySelector("meta[name='author']")?.content,
  productVersion: document.querySelector("meta[name='version']")?.content,
});

import { getDefaultContext } from './Runtime/include/emnapi.mjs';
const exports = Module.emnapiInit({ context: getDefaultContext() });
export default exports;
