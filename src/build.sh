#!/bin/bash

docker run --rm -i \
    -v /root/.nuget/packages/:/root/.nuget/packages/ \
    -v ${PWD}:/workspace \
    mcr.microsoft.com/dotnet/sdk:5.0-alpine \
    sh -c 'cd /workspace && sh publish_api.sh'

docker run --rm -i \
    -v /usr/lib/node_modules/:/usr/lib/node_modules/ \
    -v ${PWD}:/workspace \
    node:16-alpine \
    sh -c 'cd /workspace && sh publish_web.sh'