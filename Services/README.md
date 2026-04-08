# Purpose of the PharmaNet API
The PharmaNet APIs serve as a modern REST/FHIR gateway that:

- Accepts FHIR DocumentReference requests via REST endpoints
- Extracts embedded HL7v2 messages from the requests
- Routes them to external PharmaNet services (likely via HL7 messaging protocols)
- Returns FHIR-formatted responses back to clients

The authentication/authorization layer ensures proper access control, but the core functionality is indeed this translation and routing between modern web APIs and legacy HL7v2 healthcare messaging systems.

# PractitionerService
Note: other services in this solution work in a similar way to PractitionerService.

## Overview
`PractitionerService` is an ASP.NET Core Web API service within the PharmaNetAPIs suite. It exposes a secure HTTP endpoint that accepts FHIR-based requests and forwards them to the underlying Pharmanet proxy service.

This service is built on a shared service base (in `Common/src/ServiceBase`) that configures authentication, routing, Swagger, and common dependencies.

## Key Concepts

### Controller discovery & routing
This service uses standard ASP.NET Core MVC controller discovery:
- `Common/src/AspNetConfiguration/StartupConfiguration.cs` calls `services.AddControllers()`.
- `UseRest()` calls `app.UseEndpoints(routes => routes.MapControllers());`.

That means any class inheriting from `ControllerBase` (or `ServiceBaseController`) decorated with `[ApiController]` is automatically discovered and instantiated per request.

### `PractitionerController`
The controller lives in `src/Controllers/PractitionerController.cs` and exposes two POST endpoints:

- `/api/v{version:apiVersion}/Practitioner/` (main transaction endpoint)
- `/healthz` (health check endpoint)

The controller is resolved via dependency injection and requires:
- `ILogger<ServiceBaseController>`
- `IPharmanetService`
- `IHl7Parser`
- `IConfiguration`
- `IAuthorizationService`
- `IHttpContextAccessor`

Those dependencies are registered in `Common/src/ServiceBase/Startup.cs` and in `Common/src/AspNetConfiguration/StartupConfiguration.cs`.

### Security
The controller actions are protected by JWT bearer authentication and the `FhirScopesPolicy.Access` authorization policy.

## Running
The `Program.cs` entrypoint uses `ProgramConfiguration.CreateHostBuilder<Startup>(args)` to wire up the shared startup and host configuration.

To run locally (from the `src` folder):

```bash
dotnet run
```

## Useful paths
- `src/Controllers/PractitionerController.cs` - API controller implementation
- `Common/src/ServiceBase/Startup.cs` - shared DI registration (services + auth + swagger)
- `Common/src/AspNetConfiguration/StartupConfiguration.cs` - shared middleware pipeline and controller routing
