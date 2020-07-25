# MedicationRequest Resource

This resource endpoint is following  HL7 FHIR version 4.0.1 resource naming convention and represents an order for both supply of medication and the instructions for administration of the medicine to the patient. This is commonly referred to as a "prescription".  It replaces the paper-based prescription hand written or printed out and signed by a physician.

Unlike a pure RESTful interface, all HL7-v2 interactions are submitted with HTTP POST, with the Content-Type, or payload being the Base 64 encoded HL7-v2 message. The response will be an HL7-v2 message, again Base 64 encoded.

## Supported HL7-v2 Interactions

| PharmaNet Interaction | Description |  HL7-v2 Request | HL7-v2 Response |  HTTP Request Method |
| ------ | ------ | ------ | ------ | ---- |
| Retrieve patient prescription record | | TRX_X0.X5 | TRX_X0.X5_RESPONSE |  POST |
| Retrieve prescriber prescription record | | TRX_X4.X9 | TRX_X4.X9_RESPONSE | POST |
| Record Prescription | | TRX_X1.X6 | TRX_X1.X6_RESPONSE | POST |
| Change Prescription Status | | TRX_X2.X7 | TRX_X2.X7_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Retrieve patient prescription record | TRX_X0.X5 | system/MedicationRequest.read, or user/MedicationRequest.read, or patient/MedicationRequest.read |
| Retrieve prescriber prescription record | TRX_X4.X9 |  system/MedicationRequest.read, or user/MedicationRequest.read, or patient/MedicationRequest.read |
| Record Prescription | TRX_X1.X6 | system/MedicationRequest.write, or user/MedicationRequest.write |
| Change Prescription Status | TRX_X2.X7 | system/MedicationRequest.write, or user/MedicationRequest.write |
