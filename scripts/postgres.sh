#!/usr/bin/env sh
docker run --rm --name postgres \
    -e POSTGRES_PASSWORD=postgres \
    -p 5432:5432 \
    postgres:latest
