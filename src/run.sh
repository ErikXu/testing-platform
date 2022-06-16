#!/bin/bash
docker rm -f testing-platform
docker run --name testing-platform -d -e MONGO__CONNECTIONSTRING=mongodb://localhost:27017 -p 80:80 -p 8080:8080 testing-platform:1.0.0