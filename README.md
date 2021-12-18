# testing-platform

## Setup MongoDB

``` bash
mkdir -p /opt/mongo
docker run --name mongo -v /opt/mongo:/etc/mongo -p 27017:27017 -d mongo:4.2
```