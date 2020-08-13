# Location Resource

This HL7 FHIR-compliant resource endpoint providing details and position information for a physical place where services are provided and resources and participants may be stored, found, contained, or accommodated.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Location inquiry  | This transaction allows the user to retrieve the name, address, and telephone number for a specified POS location identifier or Pharmacy ID from PharmaNet. This transaction will return the name, address and telephone number for a specified POS Location Identifier. | read | TIL_00.50 | TIL_00.50_RESPONSE |  POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Location Inquiry | TIL_00.50 | system/Location.read, or user/Location.read |

## Example TIL.00.50 Request

MSH|^~\&|1234567|1234567||EMRMD|||userID:192.168.0.1|ZPN|1111|P|2.1||
ZCA||70|00|MA|01|
ZCB|MDA|120113|1111
ZPL|QAERXPP||||||||||||||MM
ZZZ|TIL||1111|P1|nnnnnnnnnn|||||ZZZ1

## The REST HL7 FHIR DocumentReference request using HTTP POST

The following illustrates an example TIL_00.50 Request message formulated into an HTTP POST containing a body of type HL7 FHIR DocumentReference JavaScript Object Notation (JSON):

```code
POST https://api.example.org/PharmaNet/LocationService/v1/Location HTTP/1.1
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
            "data": "TVNIfF5+XCZ8MTIzNDU2N3wxMjM0NTY3fHxFTVJNRHx8fHVzZXJJRDoxOTIuMTY4LjAuMXxaUE58MTExMXxQfDIuMXx8ClpDQXx8NzB8MDB8TUF8MDF8ClpDQnxNREF8MTIwMTEzfDExMTEKWlBMfFFBRVJYUFB8fHx8fHx8fHx8fHx8fE1NClpaWnxUSUx8fDExMTF8UDF8bm5ubm5ubm5ubnx8fHx8WlpaMQ=="
        }
    }]
}
```
