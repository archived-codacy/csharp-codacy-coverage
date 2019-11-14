#!/usr/bin/env bash

set -e

mkdir -p artifacts/

pushd src
	if [[ $1 == "netcoreapp3.0" ]]; then
		dotnet new tool-manifest --force
		dotnet tool install --local dotnet-warp
		dotnet warp -r "$2" -p "Configuration=Release" -p "PublishTrimmed=true" -p "UsePreviewVersion=true"
		if [[ $2 == win-* ]]; then
			BIN_EXT=".exe"
		else
			BIN_EXT=""
		fi
		mv -f "Codacy.CSharpCoverage$BIN_EXT" "../artifacts/Codacy.CSharpCoverage_$2$BIN_EXT"
	else
		dotnet publish -c Release -f $1 -r $2
		pushd bin/Release/$1/$2/publish
			BIN_OUTPUT="csharp-codacy-coverage_$2"
			if [[ $2 == linux-* ]] || [[ $2 == osx-* ]] ; then
				BIN_OUTPUT+=".tar.gz"
				tar -czvf $BIN_OUTPUT *
			else
				BIN_OUTPUT+=".zip"
				zip -r $BIN_OUTPUT *
			fi
			mv -f $BIN_OUTPUT ../../../../../../artifacts/
		popd
	fi
popd
