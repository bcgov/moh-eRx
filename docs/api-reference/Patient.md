# Patient Resource

This HL7 FHIR-compliant resource endpoint patient demographics and other administrative information about an individual receiving care or other health-related services.

The data in the Resource covers the "who" information about the patient: its attributes are focused on the demographic information necessary to support the administrative, financial and logistic procedures. A Patient record is generally created and maintained by each organization providing care for a patient. A patient receiving care at multiple organizations may therefore have its information present in multiple Patient Resources.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Patient Name Search | The patient name search is used to locate the PHN for a particular patient. | read | TPN |  |  POST |
| Assign Personal Health Number | Once the practitioner has searched their local system and PharmaNet, and is certain a PHN does not exist for a particular patient, a PHN may be assigned. Adding new PHNs is permitted. | write | TPH |  xxx | POST |
| Patient Identification Query|  | read | TID |  xxx | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Patient Name Search | TPN | system/Patient.read, or user/Patient.read |
| Assign Personal Health Number | TPH | system/Patient.write, or user/Patient.write |
| Patient Address Update | TPA | system/Patient.write, or user/Patient.write |

## Example TPN Request

TBD...

## The REST HL7 FHIR DocumentReference request using HTTP POST

