# PharmaNet Electronic Prescribing (eRx) API Architecture/Design

## Introduction

Every prescription that is dispensed in community pharmacies in British Columbia is entered into PharmaNet. The introduction of Electronic Prescribing adds that every prescription written in the community is entered into PharmaNet. Community pharmacies will "pull down" prescriptions for the patient when dispense requests are made. This eliminates the need for prescribing physicians to hand a piece of paper, often printed out and signed to the patient during a patient visit.  Instead, the prescription is sent from the prescribing practitioner's electronic medical records system to PharmaNet.

This modern API work seeks to achieve four goals:

1. To strive towards a solution for the community that allows the Ministry to retire the ageing HN-Secure private network.
2. To modernize authorization controls and ready an interaction model closer to HL7 FHIR and RESTful API constructs.
3. To leverage the PharmaNet identity platform to begin authorizing client applications but then over time, elevate the authorization to be based on PharmaNet user privilege over trusted system privilege.
4. To introduce the HL7-v2 electronic prescription service capabilities developed several years ago but never put into production.

## Approach

The approach is a pragmatic one, where we ask the vendor community to go on a journey with us, incrementally moving towards an end state of a modernized integration model with PharmaNet that is built on:

1. Highly elastic and scalable HL7 FHIR Restful microservices.
2. Uses standard [OAuth2](https://oauth.net/2/) authorization flows.
3. Is accessible via the Internet.
4. Is managed by security software and API management gateway software.
5. Has self-service vendor developer's areas to explore integration and ready for production certifications.

## API Design Guiding Principles & Constraints

- Use RESTful approach.
- Use URIs to represent resources.
- Adopt [HL7 FHIR](https://www.hl7.org/fhir) resource type conventions, resource scopes, and top-level content bundling.
- Use HTTPS POST for HL7-v2 request/response interactions as [HL7 FHIR Bundle](https://www.hl7.org/fhir/bundle.html) Resource Type
- Use Content-Type of [HL7 FHIR](https://www.hl7.org/fhir) for bundling the HL7-v2 as a binary using the mime type or Content-Type as defined in [HL7v2 over HTTP](https://hapifhir.github.io/hapi-hl7v2/hapi-hl7overhttp/")
- When a 'wet' signature is required when a prescriber submits a prescription, the FHIR Bundle will include a `signature`  for the electronic signature, specifying its mime-type as an binary image, such as `image/jpeg` in the `sigFormat` field.  In the future, the signature requirement may evolve to that of a digital signature format, such as [W3C XML Digital Signature](https://www.w3.org/Signature/Activity.html)
- Protect resource endpoints with OAuth2 using Bearer tokens (OAuth2 access tokens; aka JSON Web Token,or JWT)
- Keep HL7-v2 payload *opaque* to the resource server, with one exception: process the HL7-v2 Message Header (MSH) to ensure that the resource and scopes align to the HL7-v2 interaction, which allows access policy enforcement determined from Bearer token claims.
- Use microservice design pattern for maximum elasticity and scale; one interaction per microservice.
- APIs are self-documented using OpenAPI (fka Swagger) and will include ability to pass Bearer Token as Authorization.
- APIs will be testable/trialed using TEST environment, a base domain similar to production but using fictitious data.
- APIs will be versioned, with previous versions as part of the base URI, for example (not a real endpoint):

```bash
    https://moh.api.gov.bc.ca/PharmaNet/MedicationService/v1/
```

Some of features of the design are adopted or adapted from the [HL7 FHIR RESTful API specification](https://www.hl7.org/fhir/http.html#3.1.0).

## API Security

- All interactions must be over TLS 1.2 or higher; i.e. HTTPS.
- Authorization Header is mandatory with Mandatory OAuth2 access token as "Bearer {token}", e.g.:

    ```bash
    Authorization: Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI0YTEyNDUyZS1mNTMzLTQyOWEtYjg1Yi01YWU1YjIxMDg1ZTQiLCJleHAiOjE1ODkzMjE3MDgsIm5iZiI6MCwiaWF0IjoxNTg5MzIxNDA4LCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJoZWFsdGhnYXRld2F5Iiwic3ViIjoiOTA1ZjczM2EtYzFlZi00NTY0LWEzMjYtYzAyMTUxZTllNzcwIiwidHlwIjoiSUQiLCJhenAiOiJoZWFsdGhnYXRld2F5Iiwibm9uY2UiOiJjY2ZmNzI4Yi1hMzcyLTRhNGEtODczZC0wMzM2YjQzN2M1YzgiLCJhdXRoX3RpbWUiOjE1ODkzMjE0MDIsInNlc3Npb25fc3RhdGUiOiI1YTI2ZmYwOC01ZmZmLTQzNDMtODIyYi0yNTNlM2VlZjNlM2EiLCJhY3IiOiIxIiwiaGRpZCI6IkVYVFJJT1lGUE5YMzVUV0VCVUFKM0RORkRGWFNZVEJDNko0TTc2R1lFM0hDNUVSMk5LV1EifQ.qlBkGHsmcN0Y32gTyuaUPV0yZZSROrzlpXmDdwpbDR8
    ```

- Do not include Personally Identifiable Information in the resource URI. This is because URLs can often be logged in network traffic monitoring tooling, and or cached in browsers.  This means that the BC Personal Health number cannot be the resource identifier, since the PHN is considered PI. Use the HDID or other surrogate key.
- Leverage API Management software over whitelist or blacklist IP to control access to the APIs. 
- Where possible keep microservice routes private, only exposing to authorized services: make publicly reachable only through the API Management software (Kong).

## API Publishing

 The plan is to publish these APIs to PharmaNet vendors. To make them more broadly available the recommendation is to publish these APIs through the [BC Government API Registry]("https://catalogue.data.gov.bc.ca/group/bc-government-api-registry").

## API Management

 The design includes standing up a developer's portal using API management software [(Kong)]("https://konghq.com/community/?itm_source=website&itm_medium=nav"), which includes the following features:

- Documentation via OpenAPI (Swagger)
- Self-service dev and test client application account registration.
- Self-service API trials via OpenAPI
- Authentication
- Traffic Control
- API traffic Analytics
- Logging
- Caching

 The plan is to make these APIs publicly available, with authorization required, through the [BC Government API Gateway]("https://developer.gov.bc.ca/Developer-Tools/API-Gateway-\(powered-by-Kong-CE\)").

## Typical PharmaNet API Flow

The services exposed by the PharmaNet API flow through the architecture in a typical manner,as illustrated in the diagram:

![Typical Flow](../diagrams/out/PNet_API_Flow.png)

## API Solution Components

The API is organized into a set of microservices, based upon the HL7 FHIR Resources model. Each resource supports one or more interactions or transactions, and each of those transactions is a HL7 message Bundle containing the HL7-v2 request or response.

Each API Resource component interacts with the PharmaNet Authorization platform (Keycloak) to verify the access_token presented. The API components then communicate to the PharmaNet via a private channel to a proxy.

The electronic signatures captured for each prescription recorded, are stored in a high-available database and returned during a prescription retrieval interaction. 

![API Components](../diagrams/out/PNet_API_Components-page1.png)
![Other Components](../diagrams/out/PNet_API_Components-page2.png)


## HL7v2 Electronic Prescribing Messaging Specifications

### Interactions

An interaction is a request/response pairing, with two HL7-v2 messages contained in the Body of the HTTP request/response. The RESTful microservice implements a single Interaction, with its URI naming based on FHIR resource models.

The structure [EBNF]("https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form") of the endpoint Resource URI is:

```code
endpoint-uri  ::= 'https://' domain-name '/PharmaNet/' service-name '/' api-version '/' resource-type ;
```

Example:

```code
https:/api.example.org/PharmaNet/MedicationService/v1/MedicationStatement
```

#### Interaction Scope

For a list of interactions in scope for this API set, see [API Reference](../api-reference/README.md)

### Use of HTTP

Transmitting PharmaNet transaction over HTTPs uses the standard HTTP/1.1 protocol (RFC 2616) as a transport mechanism that will transfer the Base 64 encoded (pipe and carrot: '|^') structured HL7 message stream as an HL7 FHIR JSON Bundle entry, with a response as an HL7 FHIR JSON Bundle with the  HL7-v2 response provided in the Bundle for HTTP response codes of 2xx, or an HTTP Error with the body as generic JSON or plain text content type.

When HL7-v2 response is returned in a FHIR Bundle for any HTTP 2xx code even if there is an HL7v2 response containing an error (AE, AR, etc.) since the transport is considered to be successful. For transport and authorization errors, HTTP Error codes will be returned with `plain/text` Content-Type and any error information as JSON structure.

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
Date: Wed, 22 Jul 2020 01:43:36 GMT
Content-Type: application/fhir+json
Content-Length: 479
Connection: Closed

{
    "resourceType": "Bundle",
    "identifier": "a3168f60-e709-4a8e-b7ea-fa530e073b24",
    "type": "message",
    "timestamp": "2020-07022T01:43:36Z",
    "entry": [
        "resource": {
            "resourceType": "MessageHeader",
            "eventCoding": {
                    "code" : "TRX_X1.X6_RESPONSE",
                    "system" : "https://api.example.org/PharmaNet/hl7-v2-transactions",
                    "display" : "Record Prescription Response",
            },
            "eventUri": "",
            "source": {
                "name": "PharmaNet",
                "endpoint": "https://api.example.org/PharmaNet/RxService/v1/MedicationRequest/"
            },
            "response": {
                "identifier": "d5bec364-d2bb-11ea-87d0-0242ac130003",
                "code": "ok"
            }
        },
        "resource": {
            "resourceType": "Binary",
            "contentType": "x-application/hl7-v2+er7",
            "data": "TVNIfF5+XCZ8QXxBfEF8U0VORF9GQUNJTElUWXwyMDIwMDIxNDIxMjAwNXx8QUNLfDFmMmQ1MjQzLTFhOWEtNGE4My05ZmI5LWNlNTIzMTVmZjk2M3xUfDAuMA1NU0F8QUF8MjAxODAxMDEwMDAwMDA="
        }
    ]
  }
```

### Transport Flow

Submitting an HL7-v2 request is sent using HTTP POST. For `200 OK` response the payload, if any, wil be the HL7-v2 response returned from the PharmaNet service. The payload is opaque to the HTTPS transport - i.e. the microservice does not examine the payload.

### Content-Type

For the APIs the HTTP `Content-Type` will be `application/fhir+json` when transmitting HL7 interactions. 

For HL7-v2 contained in the FHIR Bundle, it is Base64 encoded `|^` pipe and caret HL7-v2 with `contentType` set to `x-application/hl7-v2+er7` as adopted from the HL7-v2 over HTTP recommendation, as illustrated in this example fragment:

```code
...
Content-Type: application/fhir+json
...
{
    ...
    "resource": {
        "resourceType": "Binary",
        "contentType": "x-application/hl7-v2+er7",
        "data": "TVNIfF5+XCZ8QXxBfEF8U0VORF9GQUNJTElUWXwyMDIwMDIxNDIxMjAwNXx8QUNLfDFmMmQ1MjQzLTFhOWEtNGE4My05ZmI5LWNlNTIzMTVmZjk2M3xUfDAuMA1NU0F8QUF8MjAxODAxMDEwMDAwMDA="
    },
     ...
```

The default character set, if not specified is ASCII a valid subset of UTF-8.

### Date

The Request and Response payload must provide a Date in the HTTP Header in UTC (GMT).

Syntax:

```code
 Date: <day-name>, <day> <month> <year> <hour>:<minute>:<second> GMT
```

### HTTP Response Codes

When the HL7-v2 message can be processed, the HTTP response code will be `200 OK`. This signifies that a response from PharmaNet was returned and is included in the response Bundle. Otherwise, several HTTP Response codes may be returned, most notably `401 Unauthorized`, `400 Bad Request`, or `404 Not Found` if there was an error in the route.

The overall logic for determining the response code is illustrated in this logic diagram:

![Response Code Logic](../diagrams/out/PNet_API_Processing.png)