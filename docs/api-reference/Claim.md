# Claim Resource

This HL7 FHIR-compliant resource endpoint providing a provider issued list of professional services and products which have been provided, or are to be provided, to a patient which is sent to an insurer for reimbursement.

The Claim is used by providers and payors, insurers, to exchange the financial information, and supporting clinical information, regarding the provision of health care services with payors and for reporting to regulatory bodies and firms which provide data analytics. The primary uses of this resource is to support eClaims, the exchange of information relating to the proposed or actual provision of healthcare-related goods and services for patients to their benefit payors, insurers and national health programs, for treatment payment planning and reimbursement.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Adjudicate Pharmacare Claim | The TAC (Adjudicate Claim) update transaction is used by Pharmacies to submit a claim for a dispense of a medication, a dispense of a device, or a ‘Refusal to Fill’ to PharmaNet for adjudication. | write | TACTDU_0104_REQUEST | TACTDU_5154_RESPONSE |  POST |
| Adjudicate Pharmacare Reversal | The TAC Claim Reversal transaction is used by Pharmacies to reverse a claim of a medication, a device, or a ‘Refusal to Fill’. | write | TACTDU_11_REQUEST | TACTDU_61_RESPONSE | POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Adjudicate a Dispense Claim TAC/TDU_01_04 - Input| TACTDU_0104_REQUEST | system/Claim.write, or user/Claim.write |
| Adjudicate a Dispense Claim TAC/TDU_11_61 - Input| TACTDU_11_REQUEST | system/Claim.write, or user/Claim.write |

## Example TAC/TDU_01_04 Request

TBD...

## The REST HL7 FHIR DocumentReference request using HTTP POST

The following illustrates an example TIL_00.50 Request message formulated into an HTTP POST containing a body of type HL7 FHIR DocumentReference JavaScript Object Notation (JSON):

```code
POST https://api.example.org/PharmaNet/ClaimService/v1/Claim HTTP/1.1
Date: Tue, 04 Aug 2020 21:58:33 GMT
Content-Type: application/fhir+json
Content-Length: 538
Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI4NmRlMDY4OS1iYTdkLTRjZmMtYTFmMC0wM2M0OTZiODNiNTkifQ.eyJqdGkiOiJiMjQ3N2ZhYS04ZmQ4LTQzYjYtYjk1OC1hZjdhYjJmYjQxOWIiLCJleHAiOjE1OTY1ODU2MjIsIm5iZiI6MCwiaWF0IjoxNTk2NTg1MDIyLCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJwaGFybWFuZXQiLCJzdWIiOiI5MGM2MWE3Ny1lN2RjLTRmNmItYTAxZi0wMDE5NDk3ZDM2NzUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwbmV0X3NhbXBsZV9jbGllbnQiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiI5YjU4YmMxOS1lMWQ4LTRhNDYtYThkNS1hMGNiNmI4YmYwMTIiLCJhY3IiOiIxIiwic2NvcGUiOiJzeXN0ZW0vTWVkaWNhdGlvblJlcXVlc3Qud3JpdGUgYXVkaWVuY2UiLCJjbGllbnRJZCI6InBuZXRfc2FtcGxlX2NsaWVudCIsImNsaWVudEhvc3QiOiI3MC42Ni4xNzIuMTk5IiwiY2xpZW50QWRkcmVzcyI6IjcwLjY2LjE3Mi4xOTkifQ.PvO_qE_FY3TlFWw92SCECY3dcrAObXikejzb1QecSXXtW7URlXsdD_ELg_mtjo8-TJTuJ26L-CpCwrxA_gNNQQ

{
    "resourceType": "DocumentReference",
    "masterIdentifier": "D8196F60-8E3F-40A6-B5C8-B5680B2C21EC",
    "status" : "current",
    "date": ""2020-08-13:32:12Zå",
    "content": [{
        "attachment": {
            "contentType": "x-application/hl7-v2+er7",
            "data": "TBD..."
        }
    }]
}

```
