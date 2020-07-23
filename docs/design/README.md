# BC Ministry of Health - Electronic Prescribing (eRx) API Architecture/Design

About ... TBD

## API Design Guiding Principles & Constraints

- Use RESTful approach.
- Use URIs to represent resources.
- Adopt HL7 FHIR resource naming conventions.
- Use HTTPS POST for HL7v2 request/response interactions.
- For HL7v2 interactions, set Content-Type and other HTTP-Headers from  <a href="https://hapifhir.github.io/hapi-hl7v2/hapi-hl7overhttp/">HAPI HL7v2 over HTTPS</a> with the HL7v2 Base64 encoded, as a content type extension:

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

 The plan is to publish these APIs to PharmaNet vendors. To make them more broadly available the recommendation is to publish these APIs through the <a href="https://catalogue.data.gov.bc.ca/group/bc-government-api-registry">BC Government API Registry</a>.

### API Management

 The design includes standing up a developer's portal using API managmenet software, which includes the following features:

- Documentation via OpenAPI (Swagger)
- Self-service dev and test client application account registration.
- Self-service API trials via OpenAPI
- API Rate Throttling
- API Perfomance monitoring

 The plan is to make these APIs publicly available (with authorization required) through the <a href="https://developer.gov.bc.ca/Developer-Tools/API-Gateway-(powered-by-Kong-CE)">BC Government API Gateway.</a>

## HL7v2 Electronic Prescribing Messaging Specifications

### Scope

An interation is a request/response pairing, consisting of two HL7v2 messages. The RESTful microservice implements a single Interaction, with its URI naming based on FHIR resource models.

The following PharmaNet eRx Interactionsa are in scope:

| Interaction |  Description | Request Message | Response Message |
| ------ | ------- | ------ | ------ |

### Use of HTTP

Transmitting HL7v2 over HTTPs uses the standard HTTP/1.1 protocol (RFC 2616) as a transport mechanism that can transfer the raw (pipe and carrot) structured HL7 message stream, with a response as either a Base64 encoded HL7v2 for HTTP Response code of 2xx, or an error as JSON or plain text.  When HL7v2 response is returned for any HTTP 2xx code even if there is an HL7v2 response containing an error (AE, AR, etc.) since the transport is considered to be successful. For transport and authorization errors, HTTP Error codes will be returned with plain/text Content-Type and any error information as JSON structure.

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

### HTTP Response Codes
