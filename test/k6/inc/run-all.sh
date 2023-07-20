#! /usr/bin/bash
# Runs a simple smoke test on all services based on the variables set below

# Set env to be the three-letter name of the environment (such as dev, vs1, or vc1)
# The tests invoke the APIs at https://pnet-{env}.api.gov.bc.ca
env=

# Set client to be the keycloak client id, like erx_development or ppm_development
# The dev environment requires the erx_development client, all others required ppm_development
client=

# Set secret to be the client secret associated with the client id in keycloak
secret=

# Set homedir to the local path containing the repo
# The output of the tests is saved to {homedir}/test/k6/output/{env}
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
