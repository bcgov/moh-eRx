#! /usr/bin/bash

# Runs a simple smoke test on all services based on the variables set below
env=
client=
secret=

# Path to directory containing the repo
homedir=

services=('Claim' 'Consent' 'Location' 'MedicationDispense' 'MedicationRequest' 'Medication' 'MedicationStatement' 'Patient' 'Practitioner')

for i in "${!services[@]}"; do
  echo Testing ${services[$i]}Service...
  ERX_ENV=${env} ERX_CLIENT=${client} ERX_CLIENT_SECRET=${secret} docker compose --file ${homedir}/test/k6/api/${services[$i]}/docker-compose.yml up --detach
  docker compose --file ${homedir}/test/k6/api/${services[$i]}/docker-compose.yml logs --follow > ${homedir}/test/k6/output/k6-${env}-${services[$i]}.txt

  echo Deleting ${services[$i]}Service compose stack
  docker compose --file ${homedir}/test/k6/api/${services[$i]}/docker-compose.yml rm --force
done
