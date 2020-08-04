# MedicationRequest Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 resource naming convention and represents an order for both supply of medication and the instructions for administration of the medicine to the patient. This is commonly referred to as a "prescription".  It replaces the paper-based prescription hand written or printed out and signed by a physician.


## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Retrieve patient prescription record | | read | TRX_X0.X5 | TRX_X0.X5_RESPONSE |  POST |
| Retrieve prescriber prescription record | | read | TRX_X4.X9 | TRX_X4.X9_RESPONSE | POST |
| Record Prescription | | create | TRX_X1.X6 | TRX_X1.X6_RESPONSE | POST |
| Change Prescription Status | | update | TRX_X2.X7 | TRX_X2.X7_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Retrieve patient prescription record | TRX_X0.X5 | system/MedicationRequest.read, or user/MedicationRequest.read, or patient/MedicationRequest.read |
| Retrieve prescriber prescription record | TRX_X4.X9 |  system/MedicationRequest.read, or user/MedicationRequest.read, or patient/MedicationRequest.read |
| Record Prescription | TRX_X1.X6 | system/MedicationRequest.write, or user/MedicationRequest.write, or patient/MedicationRequest.write |
| Change Prescription Status | TRX_X2.X7 | system/MedicationRequest.write, or user/MedicationRequest.write, or patient/MedicationRequest.write |

## Example Request Message

Record Prescription request and response message example with the following PNET Transaction Request, where the userId would be replaced by the userId of the authenticated prescriber.


### HL7-v2 Request Message

```code
MSH|^~\&|1234567|1234567|1234567|ERXPP|2015/03/09 10:01:01|userID:192.168.0.1|ZPN|195233|P|2.1||
ZCA|1|70|X1|AR|05|123456
ZCB|BC00007310|150301|195232
ZCC|12|1234567890|000nnnnnnnnnn|123|19690619|12345|1|QIAFCQGU|XY|000nnnnnnnnnn|M
ZPX|ZPX1^Y^91^1J7S2QMJJ^^^^^^43542368^^^20150228^^20150228^2063735^4679^PMS-BACLOFEN TAB 10MG                                           ^1500^300^1^0^750^15^001.9^3^QD^100^G^1^THIS IS A TEST^THIS IS A TEST^N^N^Y^N^Y^N^^^^PATIENT INSTRUCTIONS^PRESCRIBER NOTES^1234567890^NI^ADD^CHGD^^^^^PATIENT INSTRUCTIONS^^^^195232^
ZZZ|TRX||195232|P1|nnnnnnnnnn||THIS IS A TEST|||ZZZ1^
```

### The REST HL7 FHIR Bundle request using HTTP POST

In this request, the above HL7-v2 Request is Base-64 encoded as a FHIR Bundle entry, along with the electronic signature capture of the prescriber.

```code
POST https://example.org/PharmaNet/api/v1/MedicationRequest HTTP/1.1
Date: 
Content-Type: application/fhir+json
Content-Length: xxx
Authorization: Bearer eyJhbGciOiJSUz...

{
    "resourceType": "Bundle",
    "id": "prescription-record-example",
    "meta": {
        "lastUpdated": "2020-08-02T21:58:30Z",
    },
    "type": "message",
    
}
```

