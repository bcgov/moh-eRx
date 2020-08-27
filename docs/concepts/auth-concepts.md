# Authentication and Authorization Basics for eRx PharmaNet API

To call PharmaNet in support of electronic prescribing, your application must acquire an access token from the PharmaNet identity platform, an [OAuth2](https://oauth.net/2/) compliant identity and authorization server.  The access token contains information about your application and the permissions it has been granted to access the resources and APIs available.  To get an access token, your application must be registered with the PharmaNet identity platform.

## Access Tokens

[Access tokens](https://tools.ietf.org/html/rfc6750) issued contain information (claims) that the APIs use to validate that the calling application has the proper permissions to perform the operation it is requesting.  Your application must always transmit an access token as an HTTP Header entry, as in:

```code
POST https://api.example.org/PharmaNet/v1/RxService/MedicationRequest/ HTTP/1.1
Content-Type: application/fhir+json
Content-Length: 152
Authorization: Bearer EwAoA8l6BAAU7p9QDpi/D7xJLwsTgCg3TskyTaQAAXu71AU9f4aS4rOK5xoO/SU5HZKSXtCsDe0Pj7uSc5Ug008qTI+a9M1tBeKoTs7tHzhJNSKgk7pm5e8d3oGWXX5shyOG3cKSqgfwuNDnmmPDNDivwmi9kmKqWIC9OQRf8InpYXH7NdUYNwN+jljffvNTewdZz42VPrvqoMH7hSxiG7A1h8leOv4F3Ek/oeJX6U8nnL9nJ5pHLVuPWD0aNnTPTJD8Y4oQTp5zLhDIIfaJCaGcQperULVF7K6yX8MhHxIBwek418rKIp11om0SWBXOYSGOM0rNNN59qNiKwLNK+MPUf7ObcRBN5I5vg8jB7IMoz66jrNmT2uiWCyI8MmYDZgAACPoaZ9REyqke+AE1/x1ZX0w7OamUexKF8YGZiw+cDpT/BP1GsONnwI4a8M7HsBtDgZPRd6/Hfqlq3HE2xLuhYX8bAc1MUr0gP9KuH6HDQNlIV4KaRZWxyRo1wmKHOF5G5wTHrtxg8tnXylMc1PKOtaXIU4JJZ1l4x/7FwhPmg9M86PBPWr5zwUj2CVXC7wWlL/6M89Mlh8yXESMO3AIuAmEMKjqauPrgi9hAdI2oqnLZWCRL9gcHBida1y0DTXQhcwMv1ORrk65VFHtVgYAegrxu3NDoJiDyVaPZxDwTYRGjPII3va8GALAMVy5xou2ikzRvJjW7Gm3XoaqJCTCExN4m5i/Dqc81Gr4uT7OaeypYTUjnwCh7aMhsOTDJehefzjXhlkn//2eik+NivKx/BTJBEdT6MR97Wh/ns/VcK7QTmbjwbU2cwLngT7Ylq+uzhx54R9JMaSLhnw+/nIrcVkG77Hi3neShKeZmnl5DC9PuwIbtNvVge3Q+V0ws2zsL3z7ndz4tTMYFdvR/XbrnbEErTDLWrV6Lc3JHQMs0bYUyTBg5dThwCiuZ1evaT6BlMMLuSCVxdBGzXTBcvGwihFzZbyNoX+52DS5x+RbIEvd6KWOpQ6Ni+1GAawHDdNUiQTQFXRxLSHfc9fh7hE4qcD7PqHGsykYj7A0XqHCjbKKgWSkcAg==

```

## Register your app with the PharmaNet identity platform

Before your app can get an access token from the identity platform, it must be registered. Registration integrates your app with the identity platform and establishes the information that it uses to get tokens:

- **Application ID**:  A unique identifier assigned by the identity platform to your application.
- **Redirect URI/URL**: One or more endpoints at which your app will receive responses from the identity platform.
- **Application Secret**: A password or public/private key pair that your application uses to authenticate with the identity platform.

The first edition of the access type will allow for permissions to be granted directly to your application according to the conformance profile fo your app. So, for example, a Pharmacist's application may have a differing set of permissions than, say, a prescribing physician's electronic medical record system might have. This will be determined during registration.

Application permissions granted do not rely on signed-in user to be present; but are system-based authorization grants.  In Oauth2 standards parlance, this type of grant is referred to as "client credentials grant".

In future, the permissions determined to be granted to your application will be based on the available permissions of an authenticated user. For example, a physician creating a prescription would be directly authenticated with the PharmaNet Identity Platform from within their EMR, using OAuth2 OIDC and SSO, and then the EMR application would obtain an access token containing permissions derived from the capabilities of the physician, to effectively act on behalf of the physician to perform the prescription create request.

## Getting an access token

Our recommendation is to use existing authentication and authorization software libraries that adhere to the OAuth2 standards and support [Open ID Connect](https://openid.net/connect/), and can process access tokens and automatically add them into HTTP request headers for you with directives.

## Access Scopes

Accessing the resources and services requires that the access token supplied in the request include the necessary OAuth2 access scopes. These correspond directly to the FHIR resource types adopted in this specification. The goal is to, in future, add, FHIR R4 JSON as an accepted content-type to the resource endpoints without needing to modify the authorization controls.

### Resource Context

Simply providing an access token in the HTTP Header is not sufficient for your app to gain access to the resource endpoints and services. Your app must be assigned the appropriate access scope(s) for the given interaction. The available scopes will be determined by the nature of the authentication. For system  authentication using client credentials, your app will be pre-assigned 'system' scopes in the PharmaNet identity platform. These will align to the PharmaNet access profiles assigned to your app.

The permission type, or context component of a scope definition indicates whether the access being requested is for a single patient record or a bulk set of patient records. This specification defines two possible values.

| Context (Permission Type) | Definition |
| ----- | ------ |
| user | "User" access allows your application to access any individual resource instance that the authenticated end-user is authorized to access. The scope is for a bulk set of records or for aggregate data not representing a single patient, based on what is available to the current user. |
| patient | The scope is for a single patient's record, either for the current user or someone else that they have been given access to."Patient" access restricts your application access to only access those individual resource instances that are associated with the patient that is directly or indirectly in context.  |
| system | "System" allows an application to access a resource directly, without an authenticated user present (the access token is not supplied due to a user authenticating). This can only be utilized with the client credentials grant flow, and MUST NOT be combined with any other context. |

For the initial release of PharmaNet API with HL7-v2 over HTTP, 'system' context will be used.

For further information about Permission types, see [OpenID Smart-on-FIR topic.](https://openid.net/specs/openid-heart-fhir-oauth2-1_0-2017-05-31.html#rfc.section.2.1)

### Resource Type

The resource type of a resource scope must conform to a valid resource type as defined in the [FHIR Resource Index](http://www.hl7.org/implement/standards/FHIR/resourcelist.html)

### Modification Rights

Three modification rights are defined for a resource:

| Right | Description |
| ------ | ------ |
| read | Corresponds to "read" or "vread" and "history" for interactions as defined by [FHIR Resource Index](http://www.hl7.org/implement/standards/FHIR/resourcelist.html). For the context is "user" this includes "search". |
| write | Corresponds to "create", "update" and "delete". |
| * | Corresponds to both write and read. |

The pattern for scopes is based on the [SMART](https://openid.net/specs/openid-heart-fhir-oauth2-1_0.html) scopes, and the [EBNF](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form) notation of the scope syntax is:

```code
clinical-scope ::= ( 'patient' | 'user' | 'system' ) '/' ( resource-type | '*' ) '.' ( 'read' | 'write' | '*')
```

The following illustrates an example access_token with system access scope for submitting a prescription to PharmaNet:

```javascript
{
  "jti": "b2477faa-8fd8-43b6-b958-af7ab2fb419b",
  "exp": 1596585622,
  "nbf": 0,
  "iat": 1596585022,
  "iss": "https://sso-dev.pathfinder.gov.bc.ca/auth/realms/ff09qn3f",
  "aud": "pharmanet",
  "sub": "90c61a77-e7dc-4f6b-a01f-0019497d3675",
  "typ": "Bearer",
  "azp": "pnet_sample_client",
  "auth_time": 0,
  "session_state": "9b58bc19-e1d8-4a46-a8d5-a0cb6b8bf012",
  "acr": "1",
  "scope": "system/MedicationRequest.write audience",
  "clientId": "pnet_sample_client",
  "clientHost": "70.66.172.199",
  "clientAddress": "70.66.172.199"
}
```

## Next Steps

[Invoking PharmaNet API](invoking-pharmanet-api.md)
