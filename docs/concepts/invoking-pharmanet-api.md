# Invoking the PharmaNet API

To access the PharmaNet interaction resources, you call and specify the resource URL corresponding to the interaction your application wishes to invoke using only HTTP POST operations when submitting HL7-v2 in a FHIR Bundle over HTTPS.

> Unlike a pure RESTful interface, all HL7-v2 interactions are submitted with HTTP POST, with the content type, or mime-type set to `Content-Type: application/fhir+json`, and with top-level resource type a "message" Bundle. The HL7-v2 message is added to the FHIR Binary Message bundle, base64 encoded, with the MessageHeader eventCode set to the Request transaction code, shown in the table below.

In future, these same resource endpoints will add more HTTP actions (GET, PUT, PATCH, DELETE) when the `Content-Type` provided is HL7 FHIR, and not simply HL7-v2 bundled in a FHIR JSON wrapper.

All PharmaNet API request use the following basic URL pattern:

```code
https://api.example.org/PharmaNet/{version}/{resource}
```

Where for this example URL:

- ``https://api.example.org`` is the base domain for the PharmaNet API endpoints (this is a fictitious example).
- `{version}` is the target service version, for example the initial version will be `v1`.
- `{resource}` is the resource path, such as:
    - `MedicationRequest`, or
    - `MedicationStatement` or
    - `Medication`

## Example Request

```code
POST https://api.example.org/PharmaNet/v1/MedicationStatement/ HTTP/1.1
Content-Type: application/fhir+json
Content-Length: 147
Date: Wed, 22 Jul 2020 11:12:32 GMT
Authorization: Bearer <access_token>

{ ... HL7 FHIR Body with HL7-v2 bundled ... }
```

## Example Response

```code
HTTP/1.1 200 OK
Last-Modified: Wed, 22 Jul 2020 11:05:20 GMT
Date: Wed, 22 Jul 2020 11:12:33 GMT
Content-Type: application/fhir+json
Content-Length: 152
Connection: Closed

{ ... HL7 FHIR Body with HL7-v2 response bundled ... }
```

## Use of "Wet" Electronic Signature

An electronic signature, in the form of a binary image *must* be included in the [Prescription Record Request](../api-reference/MedicationRequest.md) Interaction.

When a 'wet' signature is required when a prescriber submits a prescription, the FHIR Bundle will include a `signature`  for the electronic signature, specifying its mime-type as an binary image, such as `image/png` in the `sigFormat` field.

For more information, see [Electronic Signatures](electronic-signatures.md).



## HL7 FHIR Bundle Concept

For information on an HL7 "message" bundle, please see [HL7 FHIR Bundle](https://www.hl7.org/fhir/bundle.html)
