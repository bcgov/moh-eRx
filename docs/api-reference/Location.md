# Location Resource

This HL7 FHIR-compliant resource endpoint providing details and position information for a physical place where services are provided and resources and participants may be stored, found, contained, or accommodated.

## Supported HL7-v2 Interactions

```javascript
"contentType": "x-application/hl7-v2+er7"
```

| PharmaNet Interaction | Description |  Type | HL7-v2 Request | HL7-v2 Response |  HTTP Request Action |
| ------ | ------ | ------ | ------ | ---- | ----- |
| Location inquiry  | | read | TIL_00.50 | TIL_00.50_RESPONSE |  POST |

## Permissions

The resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client applications has obtained the correct permissions to allow the request to be processed. If not an HTTP 40x Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART on FHIR specifications.

| PharmaNet Interaction |  Hl7-v2 Request | Required System Scope |
| ------ | ------ | ------ |
| Location Inquiry | TIL_00.50 | system/Location.read, or user/Location.read |
