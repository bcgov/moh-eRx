#!/usr/bin/env bash

# Deploys changes to a PPM API environment's infrastructure in OpenShift
#
# Usage:
#   deploy.sh COMMAND ENVIRONMENT
#
# Substitute COMMAND for the Helm command to be applied.
# Substitute ENVIRONMENT for the three-letter name of the PPM API environment to be deployed. The environments are dev, tr1, vs1, vc1, vc2, and prd.
#
# Author: Arlo Watts
# Date: 2023-30-11

helmCommand=$1
environment=$2

# Set the namespace according to the environment
if [ "$environment" = "dev" ]; then
  namespace="2f77cb-dev"

elif [ "$environment" = "tr1" ]; then
  namespace="2f77cb-test"

elif [ "$environment" = "vs1" ]; then
  namespace="2f77cb-test"

elif [ "$environment" = "vc2" ]; then
  namespace="2f77cb-test"

elif [ "$environment" = "vc1" ]; then
  namespace="d027a8-test"

elif [ "$environment" = "prd" ]; then
  namespace="d027a8-prod"
  echo "You are making changes to the live production environment of a critical health application. Type DEPLOY TO PRODUCTION to proceed."
  read confirm && [ "$confirm" = "DEPLOY TO PRODUCTION" ] || exit 1

else
  echo "\"${environment}\" is not a recognized environment."
  echo
  echo "Usage:"
  echo "  deploy.sh COMMAND ENVIRONMENT"
  echo
  echo "Substitute COMMAND for the Helm command to be applied."
  echo "Substitute ENVIRONMENT for the three-letter name of the PPM API environment to be deployed. The environments are dev, tr1, vs1, vc1, vc2, and prd."
  exit 1
fi

# Points to tools/helm
basedir=$(dirname $0)

# The list of services
services=("claim" "consent" "location" "medicationdispense" "medicationrequest" "medication" "medicationstatement" "patient" "practitioner")

# Deploy the common secrets that each PPM API service references
helm $helmCommand -n $namespace -f ${basedir}/config/common/${environment}-values.yaml common-${environment} ${basedir}/common

# Deploy the PPM API services
for service in ${services[@]}; do
  helm $helmCommand -n $namespace -f ${basedir}/config/${service}service/${environment}-values.yaml ${service}service-${environment} ${basedir}/ppmservice
done
