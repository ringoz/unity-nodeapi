import * as path from 'node:path';
import { fileURLToPath } from 'node:url';
import { dlopen, platform } from 'node:process';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const moduleName = path.dirname(path.dirname(path.dirname(__dirname)));
const exports = importAotModule(moduleName);

function importAotModule(moduleName) {
  const gamePath = platform === 'win32' ? 'GameAssembly.dll' : 'Contents/Frameworks/GameAssembly.dylib';
  const moduleFilePath = path.join(moduleName, 'Build.app', gamePath);
  const module = { exports: {} };
  dlopen(module, moduleFilePath);
  return module.exports;
}

export default exports;
export const hello = exports.hello;
