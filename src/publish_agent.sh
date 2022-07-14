#!/bin/bash

rm -rf ./publish_agent

cd Agent
dotnet publish -c Release -o ../publish_agent -r alpine-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None /p:DebugSymbols=false --self-contained true