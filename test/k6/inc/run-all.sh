#! /usr/bin/bash

# Runs a simple smoke test on all services based on the variables set below
env=
client=
secret=

# Path to directory containing the repo
homedir=

services=('Claim' 'Consent' 'Location' 'Medication' 'MedicationDispense' 'MedicationRequest' 'MedicationStatement' 'Patient' 'Practitioner')

mkdir ${homedir}/test/k6/output/${env}

for i in "${!services[@]}"; do
  echo Testing ${services[$i]}Service...

  # Set the env variables for the command
  # Compose up the test container
  # Write the output to a file
  ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} \
  docker compose --file ${homedir}/test/k6/api/${services[$i]}/docker-compose.yml up \
  > ${homedir}/test/k6/output/${env}/k6-${env}-${services[$i]}.txt 2>&1

  # Quietly delete the compose stack
  docker compose --file ${homedir}/test/k6/api/${services[$i]}/docker-compose.yml rm --force > /dev/null 2>&1
done
