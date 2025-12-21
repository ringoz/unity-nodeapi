import { getDefaultContext } from './Runtime/include/emnapi.mjs';

const { Module } = await createUnityInstance(document.querySelector("#unity-canvas"), {
  arguments: [],
  dataUrl: "Build.web.data",
  frameworkUrl: "Build.web.framework.js",
  codeUrl: "Build.web.wasm",
  streamingAssetsUrl: "StreamingAssets",
  companyName: "DefaultCompany",
  productName: "unode-module",
  productVersion: "1.0",
  // matchWebGLToCanvasSize: false, // Uncomment this to separately control WebGL canvas render size and DOM element size.
  // devicePixelRatio: 1, // Uncomment this to override low DPI rendering on high DPI displays.
});
const exports = Module.emnapiInit({ context: getDefaultContext() });

export const hello = exports.hello;
export default exports;
