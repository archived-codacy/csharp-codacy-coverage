#!/usr/bin/env bash

set -e

mkdir -p artifacts/

pushd src
	dotnet build -c Release -f $1 -r $2
	pushd bin/Release/$1/$2
		BIN_OUTPUT="csharp-codacy-coverage_$2"
		if [[ $2 == linux-* ]] || [[ $2 == osx-* ]] ; then
			BIN_OUTPUT+=".tar.gz"
			tar -czvf $BIN_OUTPUT *
		else
			BIN_OUTPUT+=".zip"
			zip -r $BIN_OUTPUT *
		fi
		mv -f $BIN_OUTPUT ../../../../../artifacts/
	popd
popd
