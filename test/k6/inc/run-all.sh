#!/bin/bash
# Runs a simple smoke test on all services based on the variables set below

# Set env to be the three-letter name of the environment (such as dev, vs1, or vc1)
# The tests invoke the APIs at https://pnet-{env}.api.gov.bc.ca
env=

# Set client to be the keycloak client id, like erx_development or ppm_development
# The dev environment requires the erx_development client, all others required ppm_development
client=

# Set secret to be the client secret associated with the client id in keycloak
secret=

# Points to test/k6
BASEDIR=$(dirname $0)/..

services=('Claim' 'Consent' 'Location' 'Medication' 'MedicationDispense' 'MedicationRequest' 'MedicationStatement' 'Patient' 'Practitioner')

mkdir ${BASEDIR}/output/${env}

for service in "${services[@]}"; do
  echo Testing ${service}Service...

  # Set the env variables for the command
  # Compose up the test container
  # Write the output to a file
  ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} \
  docker compose --file ${BASEDIR}/api/${service}/docker-compose.yml up \
  > ${BASEDIR}/output/${env}/k6-${env}-${service}.txt 2>&1

  # Quietly delete the compose stack
  docker compose --file ${BASEDIR}/api/${service}/docker-compose.yml rm --force > /dev/null 2>&1
done
