#!/bin/bash

rm -rf ./publish

cd WebApi
dotnet publish -c Release -o ../publish -r alpine-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None /p:DebugSymbols=false --self-contained true