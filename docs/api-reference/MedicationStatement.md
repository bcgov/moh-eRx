# MedicationStatement Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 specifications. It is a record of medication being taken by a patient, or that the medication has been given to a patient where the record is the result of a report from the patient, or another clinician. A medication statement is not a part of the prescribe->dispense->administer sequence but is a report that such a sequence (or at least a part of it) did take place resulting in a belief that the patient has received a particular medication.

## Supported HL7-v2 Interactions

The payload of a MedicationStatement request contains an HL7 FHIR compliant *message* Bundle, which contains a message header and a Base 64 encoded HL7-v2 messages, as listed in the table below. A successful response will contain the corresponding HL7-v2 message, also wrapped in an HL7 FHIR *message* Bundle.  


| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Patient Profile Information Request (complete) | | read | TRP_00.50 | TRP_00.50_RESPONSE |  POST |
| Patient Profile Information Request (latest) | | read | TRP_00.50 | TRP_00.50_RESPONSE |  POST |
| DUE inquiry with patient profile information (complete) | | read | TDUTRP_00.50 |TDUTRP_00.50_RESPONSE | POST |
| DUE inquiry with patient profile information (latest) | | read | TDUTRR_00.50 | TDUTRR_00.50_RESPONSE | POST |

### Bundle entry Content-Type

The HL7-v2 is base-64 encoded and must declare the following content-type:

```javascript
"contentType": "x-application/hl7-v2+er7"
```

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Patient Profile Information Request (complete or latest) | TRP_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |
| DUE inquiry with patient profile information (complete) | TDUTRP_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |
| DUE inquiry with patient profile information (latest) | TDUTRR_00.50 | system/MedicationStatement.read, or user/MedicationStatement.read, or patient/MedicationStatement.read |

## Example Request Message

```code
TBD
```
