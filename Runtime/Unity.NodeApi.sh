#!/bin/sh

pushd ../../../Build_BackUpThisFolder_ButDontShipItWithYourGame/Managed/

# enumerate all .dll files in the current directory and put them to semicolon-separated list
refs=$(find . -name "*.dll" -type f -print0 | sed 's/.\//;/g')

dotnet ~/Documents/node-api-dotnet/out/bin/Release/NodeApi.Generator/net10.0/osx-arm64/Microsoft.JavaScript.NodeApi.Generator.dll \
  --assembly "Unity.NodeApi.dll" \
  --reference "$refs" \
  --typedefs "Unity.NodeApi.d.ts" \
  --framework "netstandard2.1"

popd
