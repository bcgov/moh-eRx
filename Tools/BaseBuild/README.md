# Application Build and Deployment

Documents our OpenShift Build and Deployment process templates.

## Prerequisites

A Network Security Policy needs to be deployed into each namespace prior to anything being executed.  In order to create this, please execute the following:

```console
oc project 2f77cb-tools
oc process -f nsp.yaml -p NAMESPACE_PREFIX=2f77cb -p ENVIRONMENT=tools | oc apply -f -
oc project 2f77cb-dev
oc process -f nsp.yaml -p NAMESPACE_PREFIX=2f77cb -p ENVIRONMENT=dev | oc apply -f -
oc project 2f77cb-test
oc process -f nsp.yaml -p NAMESPACE_PREFIX=2f77cb -p ENVIRONMENT=test | oc apply -f -
oc project 2f77cb-prod
oc process -f nsp.yaml -p NAMESPACE_PREFIX=2f77cb -p ENVIRONMENT=prod | oc apply -f -
oc project 2f77cb-tools
```

Please ensure that the AzureAgents have been deployed into the OpenShift tools namespace.

### Common Secrets

The Pharmanet proxy requires 4 secrets be defined

```console
  oc process -f ./commonSecrets.yaml --parameters
```

Create the common config

```console
oc process -f ./commonSecrets.yaml -p PROXY_ENDPOINT=[ENDPOINT] -p PROXY_USERNAME=[USERNAME] -p PROXY_PASSWORD=[PASSWORD] -p PROXY_CERT_PASSWORD=[CERT PASSWORD]| oc apply -f -
```

### Certificates

The Pharmanet backing service requires a certificate for system to system authentication

```console
oc create configmap pharmanet-cert --from-file=path/cert
```

### Services

To create the services for a given namespace do the following

```console
./deploy_services.sh 2f77cb dev dev
```
