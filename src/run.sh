#!/bin/bash
docker rm -f testing-platform
docker run --name testing-platform -d -p 80:80 testing-platform:1.0.0