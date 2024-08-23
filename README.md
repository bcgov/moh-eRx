# moh-eRx

[![img](https://img.shields.io/badge/Lifecycle-Stable-97ca00)](https://github.com/bcgov/repomountie/blob/master/doc/lifecycle-badges.md)

## BC Ministry of Health PharmaNet Electronic Prescribing API

This repository holds the architecture and components to enable PharmaNet electronic prescribing in British Columbia.

## Objectives

This project has three primary objectives:

1. Migrate away from the HN-Secure private network, removing the need for proprietary HN-Client software packages.
2. Add Electronic Prescribing (eRx) capabilities using HTTPS transport and modern OAuth2 authentication and authorization standards.
3. Add support for HL7 FHIR editions of the HL7-v2 PharmaNet Interactions.

## About PharmaNet

Every prescription dispensed in community pharmacies in B.C. is entered into PharmaNet.

PharmaNet, administered by the BC Ministry of Health, was developed in consultation with health professionals and the public to improve prescription safety and support prescription claim processing.

PharmaNet users include community pharmacies, hospital pharmacies, emergency departments, hospitals, community health practices, the College of Pharmacists of British Columbia, and the College of Physicians & Surgeons of British Columbia.

For more information about PharmaNet, see https://www2.gov.bc.ca/gov/content/health/health-drug-coverage/pharmacare-for-bc-residents/pharmanet.

## The PharmaNet API

The PharmaNet API is a secure, modern and RESTful interface that allows authenticated and authorized access to all interactions available with PharmaNet. With the appropriate application profile configured by the Ministry, your application can interact with PharmaNet, allowing it to perform tasks such as submitting prescriptions and PharmaCare claims electronically. This new capability removes the need for paper-based prescriptions.

The PharmaNet API does not use the HNSecure private network and its required HNClient application. It uses modern OAuth2 standards for secure, stateless, and scalable authentication and authorization. Each transaction requires a valid OAuth2 JSON Web Token, or JWT.

Examples of applications that benefit from the use of this API include:
- Pharmacy Systems
- Clinical Pharmacy Systems
- Private and Community Practice EMR Systems

## API Resources

The PharmaNet APIs are organized by HL7 FHIR Resource Type. Each HL7 FHIR Resource type has its own PharmaNet URI, or endpoint. The Resources support one or more transactions related to the resource type and authorized access is managed by OAuth2 scopes.

### Medication Resources

- [Medication](https://github.com/bcgov/moh-eRx/wiki/Medication) is a Medication product.
- [MedicationDispense](https://github.com/bcgov/moh-eRx/wiki/MedicationDispense) is a medication dispense event record.
- [MedicationRequest](https://github.com/bcgov/moh-eRx/wiki/MedicationRequest) is a prescription for a patient.
- [MedicationStatement](https://github.com/bcgov/moh-eRx/wiki/MedicationStatement) is medication dispense history for a patient.

### Additional PharmaNet Resources

In addition to the core Medication Resources, PharmaNet supports additional resources and the corresponding HL7-v2 interactions.

- [Consent](https://github.com/bcgov/moh-eRx/wiki/Consent) is a consent directive for managing the protected word.
- [Claim](https://github.com/bcgov/moh-eRx/wiki/Claim) is claims related messaging about a prescription or dispense.
- [Location](https://github.com/bcgov/moh-eRx/wiki/Location) is location of a healthcare facility.
- [Patient](https://github.com/bcgov/moh-eRx/wiki/Patient) is information about the patient, including patient identity and demographics.
- [Practitioner](https://github.com/bcgov/moh-eRx/wiki/Practitioner) is information about a healthcare practitioner (e.g. a pharmacist or doctor).

## PharmaNet HL7-v2 Interactions

The PharmaNet system supports a large number of message-based services that are based on the HL7 2.x Standard and the Canadian Pharmacists Association's [Pharmacy Claim Standard](https://www.pharmacists.ca/products-services/pharmacy-claim-standard/). The PharmaNet R70.5 Release of PharmaNet adds HL7-v2 Interactions for electronic prescribing.

To cross-reference these HL7 version 2 interactions to the HL7 FHIR Resource URI, see [Pharmanet R70 Interactions](https://github.com/bcgov/moh-eRx/wiki/R70-Interactions).

## Permissions

Each resource endpoint examines the HL7-v2 transaction type submitted and then applies a permissions check, examining the access token provided to ensure that the calling client application has obtained the correct permissions to allow the request to be processed. If it hasn't, an HTTP 4xx Error Code will be returned, without any HL7-v2 response payload. The permission scopes are based on SMART FHIR specifications.

## Agreement and Conformance

To use the PharmaNet API, you must comply with the PharmaNet API Agreement. In addition, in order to access Production-level PharmaNet APIs, your application must pass [conformance](https://www2.gov.bc.ca/gov/content/health/practitioner-professional-resources/software/conformance-standards).

## Documentation

Information about the transactions offered by each API service and the associated HL7v2 or FHIR resources is available in the [wiki](https://github.com/bcgov/moh-eRx/wiki). The wiki also documents the GitHub Actions workflows and the sample client application contained in this project.

Information about using Helm, Kong, the API gateway, gwa, Kibana, Red Hat Advanced Cluster Security, Sysdig, and the management of the OpenShift deployments is available at the [Confluence page](https://proactionca.ent.cgi.com/confluence/display/BCMOHAM/PPM+API).
