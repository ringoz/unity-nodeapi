#!/bin/sh

dotnet ../../../../../../out/bin/Release/NodeApi.Generator/net10.0/osx-arm64/Microsoft.JavaScript.NodeApi.Generator.dll \
  --assembly "../../../Library/ScriptAssemblies/Unity.NodeApi.dll" \
  --reference "Microsoft.JavaScript.NodeApi.dll" \
  --typedefs "Unity.NodeApi.d.ts"
