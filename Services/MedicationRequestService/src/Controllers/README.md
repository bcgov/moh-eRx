The MedicationRequestController is a lightweight ASP.NET Core API controller that handles medication request transactions in the PharmaNet system. It inherits from ServiceBaseController (which contains the core logic) and exposes two endpoints:

- MedicationRequest (POST /api/v{version:apiVersion}/MedicationRequest/): Processes standard medication request transactions
- HealthCheck (POST /healthz): Performs health checks on the service

Both methods delegate to the base class's PharmanetRequest() method, which:

- Parses incoming FHIR DocumentReference requests containing HL7v2 payloads
- Performs authorization checks based on HL7 message types and user scopes
- Submits requests to the underlying PharmaNet service
- Returns FHIR-formatted responses

The incoming FHIR DocumentReference request contains a full HL7v2 message as base64-encoded data in the Attachment.Data field.

The controller is intentionally minimal since the shared functionality (parsing, authorization, service interaction) is centralized in ServiceBaseController. This follows a common pattern in this multi-service architecture where each service controller focuses on its specific routing and policies while reusing common infrastructure.