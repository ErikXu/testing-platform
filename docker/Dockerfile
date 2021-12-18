FROM alpine:3.14

RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories

RUN apk update
RUN apk add --no-cache \
        ca-certificates \
        krb5-libs \
        libgcc \
        libintl \
        libssl1.1 \
        libstdc++ \
        zlib

COPY . /app
WORKDIR /app
EXPOSE 5000

ENTRYPOINT ["/app/WebApi", "--urls=http://*:5000"]