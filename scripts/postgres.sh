#!/usr/bin/env sh
docker run --rm --name postgres \
    -e POSTGRES_PASSWORD="" \
    -p 5432:5432 \
    postgres:latest
