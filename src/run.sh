#!/bin/bash
docker rm -f test-platform
docker run --name test-platform -d -p 80:80 test-platform:1.0.0