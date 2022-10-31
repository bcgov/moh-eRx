# Helm Configuration for PPM

## Pre-requisites

You need to have the [OpenShift CLI tools](https://console.apps.gold.devops.gov.bc.ca/command-line-tools) installed
You need to have [Helm](https://helm.sh/docs/intro/install/) installed
You need to have the [GWA CLI tools](https://github.com/bcgov/gwa-cli) installed globally or available in the kong folder
You need access to the OpenShift Gold/Gold DR Clusters
You need to have the Helm environment zip expanded under the helm directory
You need to have the Kong environment zip expanded under the kong directory
You need to have the Certificate zip expanded under the helm/common directory

## OpenShift Setup

On each cluster in the tools namespace we need some one time setup to create a service account for GitHub and some role bindings.

```console
helm install -n d027a8-tools setup helm/setup
```

If this script is uninstalled and re-installed the appropriate GitHub actions secrets will have to be updated.

In each project (dev, test, prod) in Gold and Gold DR please apply the following for Kong Network Policy

```console
oc apply -n d027a8-dev -f kong/openshift/networkpolicy.yaml
oc apply -n d027a8-test -f kong/openshift/networkpolicy.yaml
oc apply -n d027a8-prod -f kong/openshift/networkpolicy.yaml
```

## Deploying

PPM is made up of common configuration/secrets and 9 services that need to be deployed on each cluster.

```console
helm install -n d027a8-dev -f helm/config/common/dev-values.yaml common-dev helm/common
helm install -n d027a8-dev -f helm/config/claimservice/dev-values.yaml claimservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/consentservice/dev-values.yaml consentservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/locationservice/dev-values.yaml locationservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/medicationdispenseservice/dev-values.yaml medicationdispenseservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/medicationrequestservice/dev-values.yaml medicationrequestservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/medicationservice/dev-values.yaml medicationservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/medicationstatementservice/dev-values.yaml medicationstatementservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/patientservice/dev-values.yaml patientservice-dev helm/ppmservice
helm install -n d027a8-dev -f helm/config/practitionerservice/dev-values.yaml practitionerservice-dev helm/ppmservice
```

A helper script is available to run each of the above commands but needs to be edited per environment.

Please edit deploy.sh and set the variables at the top

```console
environment=
ocEnvironment=
license=
helmCommand=install
```

to appropriate values then run

You can deploy more than one PPM environment into a single OpenShift project assuming sufficient quota exists.

## Kong Configuration

Review the kong/generate.sh script and run it

For each Kong environment generated extract the following variables from the xxx-pnet.env file and place into a .env file

```console
GWA_NAMESPACE=
CLIENT_ID=
CLIENT_SECRET=
GWA_ENV=prod
```

publish the environment configuration

```console
./gwa pg output/xxx-pnet.yaml
```

and repeat
