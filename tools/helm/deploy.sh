#!/bin/bash

environment=prd
ocEnvironment=prod
license="d027a8"
helmCommand=upgrade

# Points to tools/helm
BASEDIR=$(dirname $0)

services=('common' 'claimservice' 'consentservice' 'locationservice' 'medicationdispenseservice' 'medicationrequestservice' 'medicationservice' 'medicationstatementservice' 'patientservice' 'practitionerservice')

helmScripts=('common' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice' 'ppmservice')

for i in "${!services[@]}"; do
  service=${services[$i]}
  helmScript=${helmScripts[$i]}
  helm ${helmCommand} -n ${license}-${ocEnvironment} -f ${BASEDIR}/config/${service}/${environment}-values.yaml ${service}-${environment} ${BASEDIR}/${helmScript}
done
