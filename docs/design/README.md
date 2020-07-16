# BC Ministry of Health - Electronic Prescribing (eRx) API Architecture/Design

About ... TBD

## API Design Guiding Principles & Constraints

- Use RESTful approach: HTTPS request/response with POST for media-type for HL7v2 payload.
- Use URIs to represent resources; Adopt HL7 FHIR resource naming conventions.
- Use Default media-type from HAPI (HL7v2 over HTTP); accept mime_type parameter as per FHIR.
- Keep routes private; only expose routes to Kong API Management
- Protect resource endpoints with OAuth2 using Bearer tokens (access tokens; aka JWT)
- Keep HL7v2 payload opaque to the resource server:  pass-thru; all access policy enforcement is determined from Bearer token.
- Use microservice design pattern for maximum elasticity and scale; one interaction per microservice.
- APIs are self-documented using OpenAPI (fka Swagger) and will include ability to pass Bearer Token as Authorization.
- APIs will be testable/trialed using TEST environment, a base domain similar to production but using fictitious data.
- APIs will be versioned, with previous versions as part of the base URI, for example (not a real endpoint):
  ``` 
    https://moh.api.gov.bc.ca/PharmaNet/v1/
    ```

### API Security

- All interactions must be over TLS 1.2 or higher; i.e. HTTPS.
- Authorization Header is mandatory with Mandatory OAuth2 access token as "Bearer = {token}", e.g.: 

    ```javascript
    Authorization = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI0YTEyNDUyZS1mNTMzLTQyOWEtYjg1Yi01YWU1YjIxMDg1ZTQiLCJleHAiOjE1ODkzMjE3MDgsIm5iZiI6MCwiaWF0IjoxNTg5MzIxNDA4LCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJoZWFsdGhnYXRld2F5Iiwic3ViIjoiOTA1ZjczM2EtYzFlZi00NTY0LWEzMjYtYzAyMTUxZTllNzcwIiwidHlwIjoiSUQiLCJhenAiOiJoZWFsdGhnYXRld2F5Iiwibm9uY2UiOiJjY2ZmNzI4Yi1hMzcyLTRhNGEtODczZC0wMzM2YjQzN2M1YzgiLCJhdXRoX3RpbWUiOjE1ODkzMjE0MDIsInNlc3Npb25fc3RhdGUiOiI1YTI2ZmYwOC01ZmZmLTQzNDMtODIyYi0yNTNlM2VlZjNlM2EiLCJhY3IiOiIxIiwiaGRpZCI6IkVYVFJJT1lGUE5YMzVUV0VCVUFKM0RORkRGWFNZVEJDNko0TTc2R1lFM0hDNUVSMk5LV1EifQ.qlBkGHsmcN0Y32gTyuaUPV0yZZSROrzlpXmDdwpbDR8"
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

## HL7v2 Electronic Prescribing Interaction Scope

An interation is a request/response pairing, consisting of two HL7v2 messages. The RESTful microservice implements a single Interaction, with its URI naming based on FHIR resource models.

The following PharmaNet eRx Interactionsa are in scope:

| Interaction |  Description | Request Message | Response Message |
| ------ | ------- | ------ | ------ |

