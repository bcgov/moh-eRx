# Medication Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 specifications.

> Unlike a pure RESTful interface, all HL7-v2 interactions are submitted with HTTP POST, with the Content-Type, or payload being the Base 64 encoded HL7-v2 message. The response will be an HL7-v2 message, again Base 64 encoded.

## Supported HL7-v2 Interactions

```code
 Content-Type: x-application/hl7-v2+er7+b64
 ```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Drug utilization evaluation (DUE) information inquiry  | | read | TDU_00.50 | TDU_00.50_RESPONSE |  POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Drug utilization evaluation (DUE) information inquiry | TDU_00.50 | system/Medication.read, or user/Medication.read, or patient/Medication.read |
