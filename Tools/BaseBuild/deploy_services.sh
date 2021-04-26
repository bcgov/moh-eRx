#!/usr/bin/env bash
licenseplate=$1
env=$2
dotnet=$3

if [ -z "$licenseplate" ] 
then
  echo Parameter 1 must be set and is the namespace to deploy into ex: 0bd5ad
fi

if [ -z "$env" ] 
then
  echo Parameter 2 must be set and is the environment name ex: dev
fi

if [ -z "$dotnet" ] 
then
  echo Parameter 3 must be set and is the dotnet environment name ex: dev, Test, Production
fi

oc project $licenseplate-$env
oc process -f ./service.yaml -p NAME=claimservice -p APP_NAME=claimservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=consentservice -p APP_NAME=consentservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=locationservice -p APP_NAME=locationservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=medicationdispenseservice -p APP_NAME=medicationdispenseservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=medicationrequestservice -p APP_NAME=medicationrequestservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=medicationservice -p APP_NAME=medicationservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=medicationstatementservice -p APP_NAME=medicationstatementservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=patientservice -p APP_NAME=patientservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
oc process -f ./service.yaml -p NAME=practitionerservice -p APP_NAME=practitionerservice -p TOOLS_NAMESPACE=$licenseplate-tools -p ENV=$env -p ASPNETCORE_ENVIRONMENT=$dotnet | oc apply -f -
