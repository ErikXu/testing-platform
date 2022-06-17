FROM alpine:3.14

RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories

RUN apk update

# .Net dependency
RUN apk add --no-cache \
        ca-certificates \
        krb5-libs \
        libgcc \
        libintl \
        libssl1.1 \
        libstdc++ \
        zlib \
        tini

# Api test tool newman
RUN apk add nodejs npm
RUN npm install --global newman@5.3.1

# Stress test tool wrk
RUN apk add wrk
RUN apk add htop

RUN echo 'http://dl-cdn.alpinelinux.org/alpine/edge/testing' >> /etc/apk/repositories
RUN apk add --update shellinabox

COPY . /app
WORKDIR /app
EXPOSE 80

ENTRYPOINT ["/sbin/tini", "--"]
CMD ["/app/webapi", "--urls=http://*:80"]