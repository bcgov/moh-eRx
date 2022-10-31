#!/bin/bash

environments=('dev' 'test' 'prod')
environments=('dev')
for environment in "${environments[@]}"; do
    export $(grep -v '^#' $environment-pnet.env | xargs)
    ./gwa pg "output/$environment-pnet.yaml"
done