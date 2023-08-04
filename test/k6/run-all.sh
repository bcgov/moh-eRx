#!/bin/bash
# Runs a simple smoke test on all services based on the five variables set below
# Runs all services simultaneously and then captures their logs one by one as they finish

# Set env to be the three-letter name of the environment (such as dev, vs1, or vc1)
# The tests invoke the APIs at https://pnet-{env}.api.gov.bc.ca
# env must be set - it cannot be left blank
env=

# Set client to be the keycloak client id, like erx_development or ppm_development
# The dev environment requires the erx_development client, all others require ppm_development
# Defaults to erx_development
client=

# Set secret to be the client secret associated with the client id in keycloak
# secret must be set - it cannot be left blank
secret=

# Set vus to the desired number of maximum concurrent users
# See https://github.com/bcgov/moh-eRx/wiki/k6-Testing#virtual-users-vus for more info about virtual users
# Defaults to 1 - for a smoke test, 1 vu is sufficient
vus=

# Set iterations to the total desired number of iterations of the test scripts
# The number of iterations is independent of the number of virtual users
# Defaults to 1 - for a smoke test, 1 iteration is sufficient
iterations=

# Points to test/k6
BASEDIR=$(dirname $0)

services=('Claim' 'Consent' 'Location' 'Medication' 'MedicationDispense' 'MedicationRequest' 'MedicationStatement' 'Patient' 'Practitioner')

mkdir --parents ${BASEDIR}/output/${env}

# Set environment variables for docker compose stacks
export ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} ERX_VUS=${vus} ERX_ITERATIONS=${iterations}

for service in "${services[@]}"; do
  # Compose up the test container in the background - this will run all nine containers concurrently
  # Runs the container from test/k6/api/docker-componse.yml with a lowercase name
  ERX_SERVICE=${service} \
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 --file ${BASEDIR}/api/docker-compose.yml \
                 up --detach
done

for service in "${services[@]}"; do
  echo Logging ${service}Service...

  # Capture the logs of the container - logs are not read concurrently, but sequentially
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 logs --follow > ${BASEDIR}/output/${env}/k6-${env}-${service}.txt

  # Delete the compose stack after the logs are finished
  docker compose --project-name $(echo ${service} | tr '[:upper:]' '[:lower:]') \
                 rm --force
done
