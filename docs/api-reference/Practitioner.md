# Practitioner Resource

This HL7 FHIR-compliant resource representing a person who is directly or indirectly involved in the provisioning of healthcare.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Prescriber inquiry  | This function may be used to obtain information on a provider (e.g., physician, pharmacist, dentist or veterinarian) by either searching a name or by the unique identification number assigned by the appropriate regulatory body. | read | TIP_00_REQUEST | TIP_00.50_RESPONSE |  POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Location Inquiry | TIP_00_REQUEST | system/Practitioner.read, or user/Practitioner.read |

## Example TIP.00.50 Request

```code
TBD
```

## The REST HL7 FHIR DocumentReference request using HTTP POST

The following illustrates an example TIP_00 Request message formulated into an HTTP POST containing a body of type HL7 FHIR DocumentReference JavaScript Object Notation (JSON):

```code
POST https://api.example.org/PharmaNet/PractitionerService/v1/Practitioner HTTP/1.1
Date: Tue, 04 Aug 2020 21:58:33 GMT
Content-Type: application/fhir+json
Content-Length: 538
Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI4NmRlMDY4OS1iYTdkLTRjZmMtYTFmMC0wM2M0OTZiODNiNTkifQ.eyJqdGkiOiJiMjQ3N2ZhYS04ZmQ4LTQzYjYtYjk1OC1hZjdhYjJmYjQxOWIiLCJleHAiOjE1OTY1ODU2MjIsIm5iZiI6MCwiaWF0IjoxNTk2NTg1MDIyLCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJwaGFybWFuZXQiLCJzdWIiOiI5MGM2MWE3Ny1lN2RjLTRmNmItYTAxZi0wMDE5NDk3ZDM2NzUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwbmV0X3NhbXBsZV9jbGllbnQiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiI5YjU4YmMxOS1lMWQ4LTRhNDYtYThkNS1hMGNiNmI4YmYwMTIiLCJhY3IiOiIxIiwic2NvcGUiOiJzeXN0ZW0vTWVkaWNhdGlvblJlcXVlc3Qud3JpdGUgYXVkaWVuY2UiLCJjbGllbnRJZCI6InBuZXRfc2FtcGxlX2NsaWVudCIsImNsaWVudEhvc3QiOiI3MC42Ni4xNzIuMTk5IiwiY2xpZW50QWRkcmVzcyI6IjcwLjY2LjE3Mi4xOTkifQ.PvO_qE_FY3TlFWw92SCECY3dcrAObXikejzb1QecSXXtW7URlXsdD_ELg_mtjo8-TJTuJ26L-CpCwrxA_gNNQQ

{
    "resourceType": "DocumentReference",
    "masterIdentifier": {
        "system" : "urn:ietf:rfc:3986",
        "value": "urn:uuid:D8196F60-8E3F-40A6-B5C8-B5680B2C21EC"
        },
    "status" : "current",
    "date": ""2020-08-13:32:12Zå",
    "content": [{
        "attachment": {
            "contentType": "x-application/hl7-v2+er7",
            "data": "TVNIfF5+XCZ8MTIzNDU2N3wxMjM0NTY3fHxFTVJNRHx8fHVzZXJJRDoxOTIuMTY4LjAuMXxaUE58MTExMXxQfDIuMXx8ClpDQXx8NzB8MDB8TUF8MDF8ClpDQnxNREF8MTIwMTEzfDExMTEKWlBMfFFBRVJYUFB8fHx8fHx8fHx8fHx8fE1NClpaWnxUSUx8fDExMTF8UDF8bm5ubm5ubm5ubnx8fHx8WlpaMQ=="
        }
    }]
}

```
