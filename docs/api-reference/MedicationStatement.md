# MedicationStatement Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 specifications. It is a record of medication being taken by a patient, or that the medication has been given to a patient where the record is the result of a report from the patient, or another clinician. A medication statement is not a part of the prescribe->dispense->administer sequence but is a report that such a sequence (or at least a part of it) did take place resulting in a belief that the patient has received a particular medication.

> Unlike a pure RESTful interface, all HL7-v2 interactions are submitted with HTTP POST, with the Content-Type, or payload being the Base 64 encoded HL7-v2 message. The response will be an HL7-v2 message, again Base 64 encoded. 

## Supported HL7-v2 Interactions

```code
 Content-Type: x-application/hl7-v2+er7+b64
 ```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Patient Profile Information Request (complete) | | read | TRP_00.50 | TRP_00.50_RESPONSE |  POST |
| Patient Profile Information Request (latest) | | read | TRP_00.50 | TRP_00.50_RESPONSE |  POST |
| DUE inquiry with patient profile information (complete) | | read | TDUTRP_00.50 |TDUTRP_00.50_RESPONSE | POST |
| DUE inquiry with patient profile information (latest) | | read | TDUTRR_00.50 | TDUTRR_00.50_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Patient Profile Information Request (complete or latest) | TRP_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |
| DUE inquiry with patient profile information (complete) | TDUTRP_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |
| DUE inquiry with patient profile information (latest) | TDUTRR_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |