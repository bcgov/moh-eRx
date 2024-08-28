#!/bin/bash

# get the payload from the first argument
payload=$1

# select the correct keycloak token url by environment
if [ $ASPNETCORE_ENVIRONMENT = 'dev' ]; then
  tokenEndpoint='https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token'

elif [ $ASPNETCORE_ENVIRONMENT = 'tr1' ]; then
  tokenEndpoint='https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token'

elif [ $ASPNETCORE_ENVIRONMENT = 'vs1' ]; then
  tokenEndpoint='https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token'

elif [ $ASPNETCORE_ENVIRONMENT = 'vc2' ]; then
  tokenEndpoint='https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token'

elif [ $ASPNETCORE_ENVIRONMENT = 'vc1' ]; then
  tokenEndpoint='https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token'

elif [ $ASPNETCORE_ENVIRONMENT = 'prd' ]; then
  tokenEndpoint='https://common-logon.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token'

else
  exit 1
fi

# request the access token from keycloak
accessToken=$(curl --location --request POST ${tokenEndpoint} \
  --header 'Content-Type: application/x-www-form-urlencoded' \
  --data-urlencode 'grant_type=client_credentials' \
  --data-urlencode "client_id=${HEALTH_CHECK_CLIENT_ID}" \
  --data-urlencode 'audience=pharmanet' \
  --data-urlencode 'scope=openid system/*.write system/*.read system/Claim.read system/Claim.write system/Consent.read system/Consent.write system/Location.read system/Medication.read system/MedicationDispense.read system/MedicationDispense.write system/MedicationRequest.read system/MedicationRequest.write system/MedicationStatement.read system/Patient.read system/Patient.write system/Practitioner.read' \
  --data-urlencode "client_secret=${HEALTH_CHECK_CLIENT_SECRET}" 2> /dev/null | sed 's/.*"access_token":"\([0-9a-zA-Z_\-]*\.[0-9a-zA-Z_\-]*\.[0-9a-zA-Z_\-]*\)".*/\1/')

# submit a transaction to the current service
curl --silent --request POST 'https://127.0.0.1:8080/' \
  --header 'Content-Type: application/json' \
  --header "Authorization: Bearer ${accessToken}" \
  --data "{'resourceType':'DocumentReference','status':'current','date':'$(date --iso-8601=seconds)','content':[{'attachment':{'contentType':'x-application/hl7-v2+er7','data':'${payload}'}}]}"
