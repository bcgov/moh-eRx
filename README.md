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

PharmaNet, administered by the Ministry of Health, was developed in consultation with health professionals and the public to improve prescription safety and support prescription claim processing.

PharmaNet users include community pharmacies, hospital pharmacies, emergency departments, hospitals, community health practices, the College of Pharmacists of British Columbia, and the College of Physicians & Surgeons of British Columbia.

For more information about PharmaNet, see [PharmaNet](https://www2.gov.bc.ca/gov/content/health/health-drug-coverage/pharmacare-for-bc-residents/pharmanet).

## The PharmaNet API
The PharmaNet API is a secure, modern, RESTful interface that allows developers to access PharmaNet services. These APIs aim to replace the need for HNSecure networking, which is scheduled to be decommissioned by 2022.

## Agreement and Conformance
To use the PharmaNet API, you must comply with the PharmaNet API Agreement. In addition, in order to access Production-level PharmaNet APIs, your application must pass [conformance](https://www2.gov.bc.ca/gov/content/health/practitioner-professional-resources/software/conformance-standards).

## Documentation
For more information about this project, see the [documentation](docs/moh-eRx_documentation.md).
