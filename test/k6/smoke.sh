#!/bin/bash
##
## How to run this on command-line:
## ERX_PASSWORD=<password> ERX_ENV=<env> $1
## Where last argument is the k6_*.js file you want to run
##
##
## Example Run
##
## cd k6
## /bin/bash smoke.sh ./src/k6_MedicationRequest.js
##
docker run -v $PWD/src:/src -a STDOUT -a STDERR -i loadimpact/k6 run --vus 1 --iterations 1 -e ERX_CLIENT=$ERX_CLIENT -e ERX_CLIENT_SECRET=$ERX_CLIENT_SECRET -e ERX_ENV=$ERX_ENV  $1
