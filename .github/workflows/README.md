# moh-eRx CI/CD Pipeline

This code repository performs CI/CD using Github Actions.

## Environments

This project has four environments: tools, dev, test, and prod. New images are automatically deployed to the tools namespace in the Silver cluster whenever a commit is pushed to the dev branch of the repo.

## Automated Pipelines

Whenever code is pushed to the dev branch, an automated action is run for each of the nine API services. Each action performs the following steps:

1. Build the code as a .NET project, storing the build data temporarily
2. Build the Docker image from the compiled code
3. Login to the OpenShift Silver cluster
4. Push the new Docker image to the tools namespace twice with the environment tags 'dev' and 'vs1'

A deployment config is set up to run images according to their environment tag. For example, images with the tag 'dev' are run in the dev environment. This is how the tagging in the pipeline step deploys the latest build to each environment.

## Manual Pipelines

This repo includes three manual workflows:

- `build-and-deploy.yml` builds the selected services and deploys them to the vc1 (conformance) namespace in Gold and Gold DR. Inputs allow the user to build and deploy either all nine services or only one.

- `release.yml` pulls the built (and tested) images from the vc1 namespace in Gold and tags them for the prod namespace in Gold and Gold DR. This workflow requires approval before it will run. The approval step runs automatically when the workflow is requested and will hold the execution of the workflow until a reviewer has approved it.

- `wava-scan.yml` runs a WAVA/ZAP scan on the APIs in the selected environment. A ZAP scan is a security scan that will probe the API endpoints for security vulnerabilities. A report will be generated for each service and will be available under 'Artifacts' in the workflow execution. The workflow will also create an Issue for each endpoint.
