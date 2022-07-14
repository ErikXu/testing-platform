#!/bin/bash
docker rm -f testing-agent
docker run --name testing-agent -d --pid=host \
-e SERVERDOMAIN="http://localhost:5000" \
-e CLIENTADDRESS="127.0.0.1" \
-e AGENTPORT=5001 \
-e MONITORPORT=5002 \
-p 5001:5001 -p 5002:5002 \
testing-agent:1.0.0