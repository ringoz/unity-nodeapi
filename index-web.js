import { name as productName, author as companyName, version as productVersion } from '../../../package.json';
import listing from '../../index-web.json' with { type: 'json' };

const find = (ext) => `Build/${listing.find(f => f.endsWith(ext))}`;

await new Promise((resolve, reject) => {
  const el = document.createElement('script');
  el.src = find('.loader.js');
  el.onload = resolve;
  el.onerror = reject;
  document.head.appendChild(el);
});

const { Module } = await createUnityInstance(document.querySelector("#unity-canvas"), {
  arguments: [],
  dataUrl: find('.data'),
  frameworkUrl: find('.framework.js'),
  codeUrl: find('.wasm'),
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

context.openScope();
