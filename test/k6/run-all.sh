#!/bin/bash
# Runs a simple smoke test on all services based on the five variables set below
# Runs all services simultaneously and then captures their logs one by one as they finish

# Set env to be the three-letter name of the environment (such as dev, vs1, or vc1)
# The tests invoke the APIs at https://pnet-{env}.api.gov.bc.ca
env=

# Set client to be the keycloak client id, like erx_development or ppm_development
# The dev environment requires the erx_development client, all others require ppm_development
client=

# Set secret to be the client secret associated with the client id in keycloak
secret=

# Set vus to the desired number of maximum concurrent users
# See the confluence documentation for more information about virtual users
# Defaults to 1
vus=

# Set iterations to the total desired number of iterations of the test scripts
# The number of iterations is independent of the number of virtual users
# Defaults to 1, which is sufficient for a smoke test
iterations=

# Set the number of transactions per iteration
# Set to -1 to run all the test transactions for the service
# Defaults to -1, which runs all tranasction types for a smoke test
iterationLength=

# To test fewer services with this script, simply remove them from this list
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
