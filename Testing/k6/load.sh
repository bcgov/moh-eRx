#!/bin/bash
##
## How to run this on command-line:
## ERX_PASSWORD=<password> ERX_ENV=<env> $1
## Where last argument is the k6_*.js file you want to run
##
##
## Example Run
##
## cd k6/run
## /bin/bash run.sh ./src/k6_MedicationRequest.js
##
docker run -v $PWD/src:/src -a STDOUT -a STDERR -i loadimpact/k6 run --vus=100 --iterations=125 -e ERX_CLIENT=$ERX_CLIENT -e ERX_CLIENT_SECRET=$ERX_CLIENT_SECRET -e ERX_ENV=$ERX_ENV  $1 $2 $2 $4
