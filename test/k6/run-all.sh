#!/usr/bin/env bash

# Runs a smoke test (or load test) on all PPM API services and logs the results in output/<ENVIRONMENT>
#
# Usage:
#   run-all.sh ENVIRONMENT CLIENT_ID CLIENT_SECRET VUS ITERATIONS ITERATION_LENGTH
#
# Substitute ENVIRONMENT for the three-letter name of the PPM API environment to be tested. The environments are dev, tr1, vs1, vc1, vc2, and prd.
# Substitute CLIENT_ID for the Keycloak client ID associated with the given environment.
# Substitute CLIENT_SECRET for the client secret associated with the given client ID.
# Substitute VUS for the maximum number of concurrent virtual users. Defaults to 1 if not set. See the confluence documentation for more information about virtual users.
# Substitute ITERATIONS for the number of iterations to run on each service. Defaults to 1 if not set.
# Substitute ITERATION_LENGTH for the number of transactions sent in each iteration. Set to -1 to run all available test transactions on every service. Defaults to -1 if not set.
#
# Author: Arlo Watts
# Date: 2023-30-11

env=$1
client=$2
secret=$3
vus=$4
iterations=$5
iterationLength=$6

# To test fewer services with this script, remove them from this list
services=('Claim' 'Consent' 'Location' 'Medication' 'MedicationDispense' 'MedicationRequest' 'MedicationStatement' 'Patient' 'Practitioner')

BASEDIR=$(dirname $0) # Points to test/k6
mkdir --parents ${BASEDIR}/output/${env}

# Set environment variables for docker compose stacks
export ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} ERX_VUS=${vus} ERX_ITERATIONS=${iterations} ERX_ITERATION_LENGTH=${iterationLength}

for service in "${services[@]}"; do
  # Compose up the test container in the background - this will run all nine containers concurrently
  # Runs the container from test/k6/api/docker-componse.yml with a lowercase name
  ERX_SERVICE=${service} \
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 --file ${BASEDIR}/docker-compose.yml \
                 up --detach
done

for service in "${services[@]}"; do
  echo Logging ${service}Service to ${BASEDIR}/output/${env}/k6-${env}-${service}.txt

  # Capture the logs of the container - logs are not read concurrently, but sequentially
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 logs --follow > ${BASEDIR}/output/${env}/k6-${env}-${service}.txt

  # Delete the compose stack after the logs are finished
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 rm --force
done
