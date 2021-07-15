curl --location --request POST 'https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=client_credentials' \
--data-urlencode 'client_id={your client id goes here}' \
--data-urlencode 'audience=pharmanet' \
--data-urlencode 'scope=audience system/MedicationRequest.write' \
--data-urlencode 'client_secret={your client secret goes here}' --silent | jq '.access_token' | tee jwt.txt |  tr -d '"' | jq -R 'split(".") | .[1] | @base64d | fromjson' <<< "$JWT"; cat jwt.txt