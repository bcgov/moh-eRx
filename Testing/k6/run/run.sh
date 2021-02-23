#!/bin/bash
##
## How to run this on command-line:
## ERX_PASSWORD=<password> ERX_ENV=<env> $1
##
docker run -v $PWD/../src:/src -a STDOUT -a STDERR -i loadimpact/k6 run -e ERX_PASSWORD=$ERX_PASSWORD -e ERX_ENV=$ERX_ENV -e ERX_CLIENT=$ERX_CLIENT $1
