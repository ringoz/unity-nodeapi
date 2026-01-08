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

import { getDefaultContext } from './Runtime/include/emnapi.mjs';
const exports = Module.emnapiInit({ context: getDefaultContext() });
export default exports;
