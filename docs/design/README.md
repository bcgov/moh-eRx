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
- APIs will be versioned, with previous versions as part of the base URI, for example:
  ``` 
    https://api.gov.bc.ca/health/PharmaNet/v1/
    ```

### API Security

- All interactions must be over TLS 1.2 or higher; i.e. HTTPS.
- Authorization Header is mandatory with Mandatory "Bearer = "  

    ```javascript
    Authorization = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI0YTEyNDUyZS1mNTMzLTQyOWEtYjg1Yi01YWU1YjIxMDg1ZTQiLCJleHAiOjE1ODkzMjE3MDgsIm5iZiI6MCwiaWF0IjoxNTg5MzIxNDA4LCJpc3MiOiJodHRwczovL3Nzby1kZXYucGF0aGZpbmRlci5nb3YuYmMuY2EvYXV0aC9yZWFsbXMvZmYwOXFuM2YiLCJhdWQiOiJoZWFsdGhnYXRld2F5Iiwic3ViIjoiOTA1ZjczM2EtYzFlZi00NTY0LWEzMjYtYzAyMTUxZTllNzcwIiwidHlwIjoiSUQiLCJhenAiOiJoZWFsdGhnYXRld2F5Iiwibm9uY2UiOiJjY2ZmNzI4Yi1hMzcyLTRhNGEtODczZC0wMzM2YjQzN2M1YzgiLCJhdXRoX3RpbWUiOjE1ODkzMjE0MDIsInNlc3Npb25fc3RhdGUiOiI1YTI2ZmYwOC01ZmZmLTQzNDMtODIyYi0yNTNlM2VlZjNlM2EiLCJhY3IiOiIxIiwiaGRpZCI6IkVYVFJJT1lGUE5YMzVUV0VCVUFKM0RORkRGWFNZVEJDNko0TTc2R1lFM0hDNUVSMk5LV1EifQ.qlBkGHsmcN0Y32gTyuaUPV0yZZSROrzlpXmDdwpbDR8"
    ```

 - Do not include Personally Identifiable Information in the resource URI. This is becaause URLs can often be logged in network traffice monitoring tooling, and or cached in browsers.  This means that the BC Personal Health number cannot be the resource identifier, since the PHN is considered PI. Use the HDID or other surrogate key.

## HL7v2 Electronic Prescribing Interaction Scope

An interation is a request/response pairing, consisting of two HL7v2 messages. The RESTful microservice implements a single Interaction, with its URI naming based on FHIR resource models.

The following PharmaNet eRx Interactionsa are in scope:

| Interaction |  Description | Request Message | Response Message |
| ------ | ------- | ------ | ------ |

