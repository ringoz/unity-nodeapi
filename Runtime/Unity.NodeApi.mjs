let exports = {};
if (typeof process !== 'undefined') {
  const { dirname, join } = await import('node:path');
  const { fileURLToPath } = await import('node:url');
  const { dlopen, platform } = await import('node:process');
  const __filename = fileURLToPath(import.meta.url);
  const __dirname = dirname(__filename);
  const moduleName = dirname(dirname(dirname(__dirname)));
  const gamePath = platform === 'win32' ? 'GameAssembly.dll' : 'Contents/Frameworks/GameAssembly.dylib';
  const moduleFilePath = join(moduleName, 'Build.app', gamePath);
  dlopen({ exports }, moduleFilePath);
}
else {
  const { getDefaultContext } = await import('./include/emnapi.mjs');
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
  exports = Module.emnapiInit({ context: getDefaultContext() });
}

export const hello = exports.hello;
export default exports;
