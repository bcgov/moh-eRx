# PharmaNet Electronic Prescribing (eRx) API Architecture/Design

## Introduction

Every prescription that is dispensed in community pharmacies in British Columbia is entered into PharmaNet. The introduction of Electronic Prescribing adds that every prescription written in the community is entered into PharmaNet. Community pharmacies will "pull down" prescriptions for the patient when dispense requests are made. This eliminates the need for prescribing physicians to hand a piece of paper, often printed out and signed to the patient during a patient visit.  Instead, the prescription is sent from the prescriber's electronci medical records system to PharmaNet.

This modern API work seeks to acheive four goals:

1. To strive towards a solution for the community that allows the Ministry to retire the ageing HN-Secure private network.
2. To modernize authorization controls and ready an interaction model closer to HL7 FHIR and RESTful API constructs. 
3. To leverage the PharmaNet identity platform to begin authorizing client applications but hten over time, elevate the authorization to be based on PharmaNet user privilege over trusted system privilege.
4. To introduce the HL7-v2 electronic prescription service capabilities developed several years ago but never put into production.

## Approach

The approach is a pragmatic one, where we ask the vendor community to go an a journey with us, incrementally heading towards an end state modernized integration model with PharmaNet that built on:

1. Highly elastic and scalable HL7 FHIR Restful microservices.
2. Uses standard OAuth2 authorization flows.
3. Runs over the Internet
4. Is managed by security software and API management
5. Has self-service vendor developer's areas to explore integration and ready for production certifications.



## API Design Guiding Principles & Constraints

- Use RESTful approach.
- Use URIs to represent resources.
- Adopt HL7 FHIR resource naming conventions.
- Use HTTPS POST for HL7v2 request/response interactions.
- For HL7v2 interactions, set Content-Type and other HTTP-Headers from [HAPI HL7 over HTTP]("https://hapifhir.github.io/hapi-hl7v2/hapi-hl7overhttp/") with the HL7v2 Base64 encoded, as a content type extension:

```bash
  Date: Thu, 16 Jul 2020 08:12:31 GMT
  Content-Type: x-application/hl7-v2+er7+b64; charset=utf-8
```

- Protect resource endpoints with OAuth2 using Bearer tokens (OAuth2 access tokens; aka JWT)
- Keep HL7v2 payload *opaque* to the resource server:  pass-thru; all access policy enforcement is determined from Bearer token.
- Use microservice design pattern for maximum elasticity and scale; one interaction per microservice.
- APIs are self-documented using OpenAPI (fka Swagger) and will include ability to pass Bearer Token as Authorization.
- APIs will be testable/trialed using TEST environment, a base domain similar to production but using fictitious data.
- APIs will be versioned, with previous versions as part of the base URI, for example (not a real endpoint):

```bash
    https://moh.api.gov.bc.ca/PharmaNet/v1/
```

Some of features of the design are adopted or adapted from the [HL7 FHIR RESTful API specification](https://www.hl7.org/fhir/http.html#3.1.0).

### API Security

- All interactions must be over TLS 1.2 or higher; i.e. HTTPS.
- Authorization Header is mandatory with Mandatory OAuth2 access token as "Bearer {token}", e.g.:

    ```bash
    Authorization: "Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI0YTEyNDUyZS1mNTMzLTQyOWEtYjg1Yi01YWU1YjIxMDg1ZTQiLCJleHAiOjE1ODkzMjE3MDgsIm5iZiI6MCwiaWF0IjoxNTg5MzIxNDA4LCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJoZWFsdGhnYXRld2F5Iiwic3ViIjoiOTA1ZjczM2EtYzFlZi00NTY0LWEzMjYtYzAyMTUxZTllNzcwIiwidHlwIjoiSUQiLCJhenAiOiJoZWFsdGhnYXRld2F5Iiwibm9uY2UiOiJjY2ZmNzI4Yi1hMzcyLTRhNGEtODczZC0wMzM2YjQzN2M1YzgiLCJhdXRoX3RpbWUiOjE1ODkzMjE0MDIsInNlc3Npb25fc3RhdGUiOiI1YTI2ZmYwOC01ZmZmLTQzNDMtODIyYi0yNTNlM2VlZjNlM2EiLCJhY3IiOiIxIiwiaGRpZCI6IkVYVFJJT1lGUE5YMzVUV0VCVUFKM0RORkRGWFNZVEJDNko0TTc2R1lFM0hDNUVSMk5LV1EifQ.qlBkGHsmcN0Y32gTyuaUPV0yZZSROrzlpXmDdwpbDR8"
    ```

- Do not include Personally Identifiable Information in the resource URI. This is because URLs can often be logged in network traffic monitoring tooling, and or cached in browsers.  This means that the BC Personal Health number cannot be the resource identifier, since the PHN is considered PI. Use the HDID or other surrogate key.
- Leverage API Management software over whitelist or blacklist IP to control access to the APIs. 
- Where possible keep microservice routes private, only exposing to authorized services: make publicly reachable only through the API Management software (Kong).

### API Publishing

 The plan is to publish these APIs to PharmaNet vendors. To make them more broadly available the recommendation is to publish these APIs through the [BC Government API Registry]("https://catalogue.data.gov.bc.ca/group/bc-government-api-registry").

### API Management

 The design includes standing up a developer's portal using API managmenet software [(Kong)]("https://konghq.com/community/?itm_source=website&itm_medium=nav"), which includes the following features:

- Documentation via OpenAPI (Swagger)
- Self-service dev and test client application account registration.
- Self-service API trials via OpenAPI
- Authentication
- Traffic Control
- API traffic Analytics
- Logging
- Caching

 The plan is to make these APIs publicly available, with authorization required, through the [BC Government API Gateway]("https://developer.gov.bc.ca/Developer-Tools/API-Gateway-\(powered-by-Kong-CE\)").

## HL7v2 Electronic Prescribing Messaging Specifications

### Interactions

An interaction is a request/response pairing, with two HL7-v2 messages contained in the Body of the HTTP request/response. The RESTful microservice implements a single Interaction, with its URI naming based on FHIR resource models.

The structure [EBNF]("https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form") of the endpoint Resource URI is:

```code
endpoint-uri  ::= 'https://' domain-name '/PharmaNet/' api-version '/' resource-type ;
```

Example:

```code
https://moh.api.gov.bc.ca/PharmaNet/v1/MedicationStatement
```

The following PharmaNet eRx Interactions are in scope:

| Interaction | HL7-v2 Request Message | Hl7-v2 Response Message |  Resource-Type |
| ------ | ------- | ------ | ------ |
| Location Inquiry | TIL_00.50 | TIL_00.50_RESPONSE | TBD |
| Retrieve Patient Prescription | TRX_X0.X5 | TRX_X0.X5_RESPONSE | MedicationRequest |
| Retrieve Prescriber Prescription Record | TRX_X4.X9 | TRX_X4.X9_RESPONSE | MedicationRequest |
| Record Prescription | TRX_X1.X6 |TRX_X1.X6_RESPONSE | MedicationRequest |

### Use of HTTP

Transmitting HL7-v2 over HTTPs uses the standard HTTP/1.1 protocol (RFC 2616) as a transport mechanism that can transfer the Base 64 encoded (pipe and carrot: '|^') structured HL7 message stream as the body of the HTTP request, with a response as either a Base 64 encoded HL7v2 in the response body for HTTP response codes of 2xx, or an HTTP Error with the body as JSON or plain text.  

When HL7v2 response is returned for any HTTP 2xx code even if there is an HL7v2 response containing an error (AE, AR, etc.) since the transport is considered to be successful. For transport and authorization errors, HTTP Error codes will be returned with plain/text Content-Type and any error information as JSON structure.

Example OAuth2 error:

```code
HTTP/1.1 401 Unauthorized
Last-Modified: Thu, 16 Jul 2020 08:12:20 GMT
Date: Thu, 16 Jul 2020 08:12:31 GMT
Content-Type: application/json
Content-Length: 105
Connection: Closed

{
    State = "AuthenticationFailed",
    Message = "Lifetime validation failed. The token is expired."
}
```

Example 200 OK Response containing an HL7-v2 ACK:

```code
HTTP/1.1 200 OK
Last-Modified: Wed, 22 Jul 2020 11:05:20 GMT
Date: Wed, 22 Jul 2020 11:12:33 GMT
Content-Type: x-application/hl7-v2+er7+b64; charset=utf-8
Content-Length: 152
Connection: Closed

TVNIfF5+XCZ8QXxBfEF8U0VORF9GQUNJTElUWXwyMDIwMDIxNDIxMjAwNXx8QUNLfDFmMmQ1MjQzLTFhOWEtNGE4My05ZmI5LWNlNTIzMTVmZjk2M3xUfDAuMA1NU0F8QUF8MjAxODAxMDEwMDAwMDA=
```

### Transport Flow

Submitting an HL7v2 request is sent using HTTP POST. For 200 OK response the payload, if any, wil be the HL7v2 response returned from the PharmaNet service. The payload is opaque to the HTTPS transport - i.e. the microservice does not examine the payload.

### Content-Type

For HL7v2 Payload with Base64 encoded "|^" vertical bar delimiting, the Content-Type HTTP Header shall be:

```javascript
Content-Type: x-application/hl7-v2+er7+b64; charset=utf-8
```

The default character set, if not specified is ASCII a valid subset of UTF-8.

### Date

The Request and Response payload must provide a Date in the HTTP Header in UTC (GMT).

Syntax:

```
 Date: <day-name>, <day> <month> <year> <hour>:<minute>:<second> GMT
```

### Custom Headers

Additional optional custom headers, adapted from HL7 FHIR recommendations are primarily intended to assist with audit/debugging of interactions.

The request id in X-Request-Id is purely to help connect between requests and logs/audit trails. The client can assign an id to the request, and send that in the X-Request-Id header. The server can either use that id or assign it's own, which it returns as the X-Request-Id header in the response. When the server assigned id is different to the client assigned id, the server SHOULD also return the X-Correlation-Id header with the client's original id in it.

The PharmaNet API endpoints will only respond with these custom headers if the client provides the optional X-Request-Id.

#### X-Request-Id

A unique id (suggest UUID) for the HTTP request/response assigned by the client and server.  Request: assigned by the client. Response: assigned by the PharmaNet API endpoint.

#### X-Correlation-Id

A client assigned request id echoed back to the client from the PharmaNet API endpoint in the Response Header. This allows the client application to correlate the response to the request.

### HTTP Response Codes
