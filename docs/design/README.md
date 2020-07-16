# BC Ministry of Health - Electronic Prescribing (eRx) API Development Project

About ... TBD

## API Design Guiding Principles & Constraints

- Use RESTful approach: HTTPS request/response with POST for media-type for HL7v2 payload.
- Use URIs to represent resources; Adopt HL7 FHIR resource naming conventions.
- Use Default media-type from HAPI (HL7v2 over HTTP); accept mime_type parameter as per FHIR.
- Keep routes private; only expose routes to Kong API Management
- Protect resource endpoints with OAuth2 using Bearer tokens (access tokens; aka JWT)
- Keep HL7v2 payload opaque to the resource server:  pass-thru; all access policy enforcement is determined from Bearer token.
- Use microservice design pattern for maximum elasticity and scale; one interaction per microservice.

## Microservices Design

