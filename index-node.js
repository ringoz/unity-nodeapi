import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { dlopen, platform } from 'node:process';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const moduleName = dirname(dirname(__dirname));
const gamePath = platform === 'win32' ? 'GameAssembly.dll' : 'Contents/Frameworks/GameAssembly.dylib';
const moduleFilePath = join(moduleName, 'Build.app', gamePath);

const exports = {};
dlopen({ exports }, moduleFilePath);
export default exports;
