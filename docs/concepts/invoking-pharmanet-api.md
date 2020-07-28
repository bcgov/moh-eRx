# Invoking the PharmaNet API

To access the PharmaNet interaction resources, you call and specify the resource URL corresponding to the interaction your application wishes to invoke using only HTTP POST operations when submitting HL7-v2 over HTTPS.

In future, these same resource endpoints will add new HTTP actions (GET, PUT, PATCH, DELETE) when the Content-Type provided is HL7 FHIR.

All PharmaNet API request use the following basic URL pattern:

```code
https://moh.api.gov.bc.ca/{version}/{resource}
```

Where for this URL:

- ``https://moh.api.gov.bc.ca`` is the base PharmaNet API endpoint (this is an example, actual endpoint may vary).
- `{version}` is the target service version, for example the initial version will be `v1`.
- `{resource}` is the resource path, such as:
    - `MedicationRequest`, or `MedicationStatement`

## Example Request

```code
POST https://moh.api.gov.bc.ca/v1/MedicationStatement/ HTTP/1.1
Content-Type: x-application/hl7-v2+er7+b64
Content-Length: 147
X-Request-Id: 610a595b-cb68-402d-b509-3d1b83066660
Date: Wed, 22 Jul 2020 11:12:32 GMT
Authorization: Bearer <access_token>

<hl7-v2-base64-encoded request body>
```

## Example Response

```code
HTTP/1.1 200 OK
Last-Modified: Wed, 22 Jul 2020 11:05:20 GMT
Date: Wed, 22 Jul 2020 11:12:33 GMT
Content-Type: x-application/hl7-v2+er7+b64; charset=utf-8
Content-Length: 152
X-Correlation-Id: 610a595b-cb68-402d-b509-3d1b83066660
Connection: Closed

<hl7-v2-base64-encoded response body>
```
