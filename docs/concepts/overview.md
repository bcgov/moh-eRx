# Overview of PharmaNet APIs

The PharmaNet APIs are the gateway to PharmaNet, the provincially operated pharmacy transaction network and system. 

## What's in the PharmaNet APIs

The PharmaNet APIs expose a set of REST APIs to access information and services through a collection of transactions:

- PharmaNet Medication services: 
- PharmaNet Electronic Prescribing services:
- PharmaNet Monograph and DUE services:
- PharmaNet Claims services:

## What can you do with PharmaNet APIs

Targeted to community practice and community pharmacies, these REST APIs not only modernize the transport and authentication and authorization aspects, but also introduce electronic prescribing capabilities allowing the prescriber to submit a prescription to PharmaNet and have it available to the patient to direct a dispense request at a pharmacy. 

This flow includes the mandatory inclusion of the prescribing practitioners '[wet signature](https://www.cpsbc.ca/for-physicians/college-connector/2014-V02-02/05)', or electronic signature, as a binary image, such as JPEG, or PNG. By employing HL7 FHIR standard, the signature is includes in the interaction alongside the HL7-v2 Record Prescription message.

## When to use the PharmaNet APIs

PharmaNet APIs provide the *new way* to interact with PharmaNet services. It is an essential way to connect to PharmaNet services using modern secure authentication and authorizations over the Internet. It replaces the ageing connection with HN-Secure private network.

In addition, because these APIs are based on standards, and adopt many of the modern Health Level 7 FHIR standards, while beginning practically with a first edition that leverages the already built HL7-v2 message interactions. This is a benefit for those client applications that currently connect using HL7-v2 messaging. Your migration to this modern API is mostly about adopting the REST APIs with OAuth2 authorization standards.

