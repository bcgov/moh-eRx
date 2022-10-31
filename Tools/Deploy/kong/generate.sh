#!/bin/bash

environments=('dev' 'test' 'prod')
environments=('dev' 'vs1')

mkdir -p output

for environment in "${environments[@]}"; do
  cat <<EOF > output/$environment-pnet.yaml
_format_version: "1.1"
services:
EOF

  #source "$environment-pnet.env"
  export $(grep -v '^#' $environment-pnet.env | xargs)

  services=('claim' 'consent' 'location' 'medicationdispense' 'medicationrequest'
            'medication' 'medicationstatement' 'patient' 'practitioner')
  serviceNames=('Claim' 'Consent' 'Location' 'MedicationDispense' 'MedicationRequest'
            'Medication' 'MedicationStatement' 'Patient' 'Practitioner')
  for i in "${!services[@]}"; do
    export SERVICE="${services[$i]}service"
    export BASE_PATH="/api/v1/${serviceNames[$i]}"
    export SWAGGER_PATH="/docs/${services[$i]}service"
    #export BASE_PATH="/${service}"
    
    MSYS_NO_PATHCONV=1 envsubst < config.tmpl >> output/$environment-pnet.yaml
    
  done
done
