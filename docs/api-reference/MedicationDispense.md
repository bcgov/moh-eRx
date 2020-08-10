# MedicationDispense Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 specifications. Indicates that a medication product is to be or has been dispensed for a named person/patient. This includes a description of the medication product (supply) provided and the instructions for administering the medication. The medication dispense is the result of a pharmacy system responding to a medication order.

## Supported HL7-v2 Interactions

The payload of a MedicationStatement request contains an HL7 FHIR compliant DocumentReference, which contains the HL7-v2 message, Base 64 encoded, as listed in the table below. A successful response will contain the corresponding HL7-v2 message, also wrapped in an HL7 FHIR DocumentReference.  


| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |

### HL7 FHIR DocumentReference entry Content-Type

The HL7-v2 is base-64 encoded and must declare the following content-type:

```javascript
"contentType": "x-application/hl7-v2+er7"
```

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |

## Example Request Message

```code
TBD
```