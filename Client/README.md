# PharmaNet API Client Example

This project is an example implementation of a client for the PharmaNet API. The PharmaNet API provides access to pharmaceutical data and services, and this client demonstrates how to authenticate and interact with it.

## Overview

The primary focus of this example is on setting up authorization, which is often the most challenging aspect of integrating with the PharmaNet API. Once authenticated, making API calls becomes straightforward. The code includes:

- **Authentication Service**: Handles OAuth2/OpenID Connect flows to obtain access tokens
- **JWT Handling**: Manages JSON Web Tokens for secure communication
- **API Integration**: Demonstrates how to make calls to PharmaNet endpoints
- **FHIR Support**: Includes message formatting for FHIR (Fast Healthcare Interoperability Resources) standard

## Prerequisites

- .NET 6.0 or later
- Access to PharmaNet API credentials (client ID, client secret, etc.)
- Valid certificates for JWT signing (see `keygen.sh` for key generation)

## Setup

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd PharmaNetAPIs/Client
   ```

2. **Configure settings**:
   - Update `src/appsettings.json` with your PharmaNet API credentials
   - Ensure certificate paths are correctly set in the configuration

3. **Generate keys** (if needed):
   ```bash
   cd src
   ./keygen.sh
   ```

4. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

5. **Build the project**:
   ```bash
   dotnet build
   ```

## Usage

Run the application:

```bash
dotnet run --project src/Client.csproj
```

The application will:
1. Authenticate with the PharmaNet API using the configured credentials
2. Obtain an access token
3. Demonstrate making API calls to PharmaNet services

## Project Structure

- `src/Program.cs`: Main entry point
- `src/Models/`: Data models for API responses and configuration
  - `AccessTokenResponse.cs`: Token response structure
  - `JwtResponse.cs`: JWT response handling
  - `OidcConfiguration.cs`: OpenID Connect configuration
  - `FHIRMessageFormat.cs`: FHIR message formatting
- `src/Services/`: Core business logic
  - `AuthService.cs`: Authentication implementation
  - `PharmanetService.cs`: PharmaNet API integration
  - `PostService.cs`: HTTP POST operations
- `src/appsettings.json`: Configuration file
- `src/keygen.sh`: Script for generating cryptographic keys

## Configuration

Key settings in `appsettings.json`:

- `PharmaNet`: API endpoints and credentials
- `JwtSigning`: Certificate configuration for JWT signing
- `OpenIdConnect`: OIDC provider settings

## Troubleshooting

- **Authentication failures**: Verify credentials and certificate validity
- **API errors**: Check network connectivity and API endpoint URLs
- **Certificate issues**: Ensure certificates are properly installed and paths are correct

## Contributing

This is an example project. For production use, ensure proper security practices, error handling, and logging are implemented.

## License

[Specify license if applicable]