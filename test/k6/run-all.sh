#!/usr/bin/env bash

# Runs a smoke test (or load test) on all PPM API services and logs the results in output/$ENVIRONMENT
#
# Usage:
#   run-all.sh ENVIRONMENT CLIENT_ID CLIENT_SECRET [VUS] [ITERATIONS] [ITERATION_LENGTH] [MAX_DURATION]
#
# Substitute ENVIRONMENT for the three-letter name of the PPM API environment to be tested. The environments are dev, tr1, vs1, vc1, vc2, and prd.
# Substitute CLIENT_ID for the Keycloak client ID associated with the given environment.
# Substitute CLIENT_SECRET for the client secret associated with the given client ID. Set to "stdin" to read the secret from standard input rather than command line arguments.
# Substitute VUS for the maximum number of concurrent virtual users. Defaults to 1 if not set. See the confluence documentation for more information about virtual users.
# Substitute ITERATIONS for the number of iterations to run on each service. Defaults to 1 if not set.
# Substitute ITERATION_LENGTH for the number of transactions sent in each iteration. Set to -1 to run all available test transactions on every service. Defaults to 1 if not set.
# Substitute MAX_DURATION for the maximum duration of the test. Defaults to "10m" (10 minutes) if not set.
#
# Author: Arlo Watts
# Date: 2024-08-14

env=$1
client=$2
secret=$3
vus=$4
iterations=$5
iterationLength=$6
maxDuration=$7

if [ "$secret" = "" ]; then
  echo "Usage:"
  echo "  run-all.sh ENVIRONMENT CLIENT_ID CLIENT_SECRET [VUS] [ITERATIONS] [ITERATION_LENGTH] [MAX_DURATION]"
  echo
  echo "Substitute ENVIRONMENT for the three-letter name of the PPM API environment to be tested. The environments are dev, tr1, vs1, vc1, vc2, and prd."
  echo "Substitute CLIENT_ID for the Keycloak client ID associated with the given environment."
  echo "Substitute CLIENT_SECRET for the client secret associated with the given client ID. Set to \"stdin\" to read the secret from standard input rather than command line arguments."
  echo "Substitute VUS for the maximum number of concurrent virtual users. Defaults to 1 if not set. See the confluence documentation for more information about virtual users."
  echo "Substitute ITERATIONS for the number of iterations to run on each service. Defaults to 1 if not set."
  echo "Substitute ITERATION_LENGTH for the number of transactions sent in each iteration. Set to -1 to run all available test transactions on every service. Defaults to 1 if not set."
  echo "Substitute MAX_DURATION for the maximum duration of the test. Defaults to \"10m\" (10 minutes) if not set."
  exit 1

elif [ "$secret" = "stdin" ]; then
  echo -n "Enter the client secret: "
  read -s secret
  echo

fi

# To test fewer services with this script, remove them from this list
services=('claim' 'consent' 'location' 'medication' 'medicationdispense' 'medicationrequest' 'medicationstatement' 'patient' 'practitioner')

# Points to test/k6
BASEDIR=$(dirname $0)

mkdir --parents ${BASEDIR}/output/${env}
rm --force ${BASEDIR}/output/${env}/*

# Create an empty file for the full test summary
touch ${BASEDIR}/output/${env}/summary.txt

# Set environment variables for docker compose stacks
export ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} ERX_VUS=${vus} ERX_ITERATIONS=${iterations} ERX_ITERATION_LENGTH=${iterationLength} ERX_MAX_DURATION=${maxDuration}

for service in "${services[@]}"; do
  # Compose up the test container in the background - this will run all nine containers concurrently
  # Runs the container from test/k6/api/docker-componse.yml
  ERX_SERVICE=${service} docker compose --project-name ${service} --file ${BASEDIR}/docker-compose.yml up --detach
done

for service in "${services[@]}"; do
  echo Logging ${service} to ${BASEDIR}/output/${env}/${service}.txt

  # Capture the logs of the container - logs are not read concurrently, but sequentially
  docker compose --project-name ${service} logs --follow > ${BASEDIR}/output/${env}/${service}.txt

  # Delete the compose stack after the logs are finished
  docker compose --project-name ${service} rm --force

  # Append the container test summary to the overall test summary
  echo "==> ${BASEDIR}/output/${env}/${service}.txt <==" >> ${BASEDIR}/output/${env}/summary.txt
  sed --silent '/^.\{44\}:/p' ${BASEDIR}/output/${env}/${service}.txt >> ${BASEDIR}/output/${env}/summary.txt
  echo >> ${BASEDIR}/output/${env}/summary.txt
done
