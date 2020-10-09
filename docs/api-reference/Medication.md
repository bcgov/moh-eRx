# Medication Resource

This resource endpoint is following HL7 FHIR version 4.0.1 specifications.

This resource is primarily used for the identification and definition of a medication for the purposes of prescribing, dispensing, and administering a medication as well as for making statements about medication use.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Drug utilization evaluation (DUE) information inquiry  | | read | TDU_00.50 | TDU_00.50_RESPONSE |  POST |
| Drug Monograph Information inquiry | The Drug Monograph Information request allows information to be requested about specific drugs based on the search criteria defined below. The transaction response returns one match that satisfies the search criteria. Along with the basic drug identification data, generic equivalents information, drug monographs, drug interaction monographs, and professional or patient education information can also be requested.| read | TDR_00_REQUEST | TDR_50_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Drug utilization evaluation (DUE) information inquiry | TDU_00.50 | system/Medication.read, or user/Medication.read, or patient/Medication.read |
| Drug Monograph Information | TDR_00.50 | system/Medication.read, or user/Medication.read, or patient/Medication.read |
