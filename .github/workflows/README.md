# moh-eRx CI/CD Pipeline

This code repository performs CI/CD using Github Actions.

## Automated pipelines

Whenever code is pushed to the dev branch, an automated action is run for each of the nine API services. Each action performs the following steps:

1. Build the code as a .NET project, storing the build data temporarily
2. Build the Docker image from the compiled code
3. Login to the OpenShift Silver cluster
4. Push the new Docker image to the tools namespace in the Silver cluster

## Environments

This project has four environments: tools, dev, test, and prod. New images are automatically deployed to the tools namespace in the Silver cluster.
