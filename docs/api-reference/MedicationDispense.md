# MedicationDispense Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 specifications. Indicates that a medication product is to be or has been dispensed for a named person/patient. This includes a description of the medication product (supply) provided and the instructions for administering the medication. The medication dispense is the result of a pharmacy system responding to a medication order.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

The payload of a MedicationStatement request contains an HL7 FHIR compliant DocumentReference, which contains the HL7-v2 message, Base 64 encoded, as listed in the table below. A successful response will contain the corresponding HL7-v2 message, also wrapped in an HL7 FHIR DocumentReference.  

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Medication Update | This transaction is used to update a PharmaNet medication profile with the dispense of a sample medication or any dispense of medication that the practitioner sees pertinent to the patient’s medical history. | write | TMU_01.51 | TMU_01.51_RESPONSE | POST |
| Medication Update Reversal | This transaction is used to correct a medication update made in error. | write | TMU_11.61 | TMU_11.61_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Medication Update | TMU_01.51 | system/MedicationDispense.write or user/MedicationDispense.write |
| Medication Update Reversal | TMU_11.61 | system/MedicationDispense.write or user/MedicationDispense.write |

## Example Request Message

```code
TBD
```
