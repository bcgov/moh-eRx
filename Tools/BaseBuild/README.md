# Health eRx Application Build and Deployment

Documents our OpenShift Build and Deployment process templates.

## Base Build

Creates a Docker based hybrid build along with the associated Image Stream which will be required for each of our configured applications.  These templates are integrated into our Azure Build Pipelines and any change will be reflected in the next build.

### Usage

To review the parameters execute:

```console
oc process -f ./build.yaml --parameters
```

To create the Build, be in your tools project and minimally execute:

```console
oc process -f ./build.yaml -p NAME=testbld | oc apply -f -
```

In your Application folder, create a base Dockerfile

```console
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY . .
#Additional application specific docker steps
```

and finally run the build from your App folder

```console
oc start-build testbld --from-dir . --follow
```

## Common Config

Creates the common configuration needed for each environment

### Typical Usage

```console
oc process -f ./common.yaml -p AUTH_OIDC_AUDIENCE=audience AUTH_OIDC_AUTHORITY=https://sso AUTH_OIDC_CLIENTSECRET=secret | oc apply -f -
```

### Secondary Usage

If you have more than one environment in a namespace, you'll need to pass in the name parameter to uniquely identify the config.

```console
oc process -f ./common.yaml -p NAME=common-secondary -p AUTH_OIDC_AUDIENCE=audiencey AUTH_OIDC_AUTHORITY=https://sso AUTH_OIDC_CLIENTSECRET=secret | oc apply -f -
```

### Deployment Script

Deploys the application throughout all environments using default parameters.

```console
./deploy_services.sh
```
