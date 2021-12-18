#!/bin/bash

rm -rf ./publish/web
mkdir -p ./publish/web

cd web

npm install
npm run build:prod

mv dist/* ../publish/web