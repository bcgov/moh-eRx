# Application Build and Deployment

Documents our OpenShift Build and Deployment process templates.

## Prerequisites

A Network Security Policy needs to be deployed into each namespace prior to anything being executed.  In order to create this, please execute the following:

```console
oc project 2f77cb-tools
oc process -f ./nsp.yaml -p NAMESPACE=2f77cb-tools | oc apply -f -
oc project 2f77cb-dev
oc process -f ./nsp.yaml -p NAMESPACE=2f77cb-dev | oc apply -f -
oc project 2f77cb-test
oc process -f ./nsp.yaml -p NAMESPACE=2f77cb-test | oc apply -f -
oc project 2f77cb-prod
oc process -f ./nsp.yaml -p NAMESPACE=2f77cb-prod | oc apply -f -
```

Please ensure that the AzureAgents have been deployed into the OpenShift tools namespace.
