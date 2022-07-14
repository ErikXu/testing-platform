#!/bin/bash

IMAGE_NAME=${IMAGE_NAME:-testing-base}
echo "IMAGE_NAME: "$IMAGE_NAME

IMAGE_TAG=${IMAGE_TAG:-latest}
echo "IMAGE_TAG: "$IMAGE_TAG


docker build --no-cache --disable-content-trust=true -t $IMAGE_NAME:${IMAGE_TAG} -f ../docker/Dockerfile_Base .
#docker push ${IMAGE_NAME}:${IMAGE_TAG}
#docker rmi ${IMAGE_NAME}:${IMAGE_TAG}