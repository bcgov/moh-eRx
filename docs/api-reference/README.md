# PharmaNet eRX REST API v1 Reference

Welcome to the PharmaNet REST API reference.

## Common Use Cases

## PharmaNet Medication Resources

The PharmaNet APIs are organized by resource type. Each HL7 FHIR Resource type has its own URI, or endpoint. The Resources support one or more transactions related to the resource type and authorized access is managed by OAuth2 scopes.

- [Medication](Medication.md)
- [MedicationDispense](MedicationDispense.md)
- [MedicationRequest](MedicationRequest.md)
- [MedicationStatement](MedicationStatement.md)

## Additional PharmaNet Resources

In addition to the core Medication Resources, PharmaNet supports additional resources and the corresponding HL7-v2 interactions.

- [Claim](Claim.md)
- [Location](Location.md)
- [Practitioner](Practitioner.md)

## PharmaNet Private Practice Interactions for ePrescribing

| Domain | Interaction Message | Description | FHIR Resource |
| ----- | ----- | ----- | ----- |
| PNet | TIL_00.50 | Location inquiry | [Location](Location.md) |
| PNet | TIL_00.50_RESPONSE | Location inquiry response | [Location](Location.md) |
| PNet | TRX_X0.X5 | Retrieve patient prescription record | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X0.X5_RESPONSE | Retrieve patient prescription record response | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X4.X9 | Retrieve prescriber prescription record | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X4.X9_RESPONSE | Retrieve prescriber prescription record response | [MedicationRequest](MedicationRequest.md) |
| PNet | TRP_00.50 | Patient profile information (complete) | [MedicationStatement](MedicationStatement.md) |
| PNet | TRP_00.50_RESPONSE | Patient profile information (complete) response | [MedicationStatement](MedicationStatement.md) |
| PNet | TRR_00.50 | Patient profile information (latest) | [MedicationStatement](MedicationStatement.md) |
| PNet | TRR_00.50_RESPONSE | Patient profile information (latest) response | [MedicationStatement](MedicationStatement.md) |
| PNet | TDR_00.50 | Drug information and monograph | [Medication](#interactions) |
| PNet | TDR_00.50_RESPONSE | Drug information and monograph response | [Medication](#interactions) |
| PNet | TPM_00.50 | Patient profile mailing request | [MedicationStatement](MedicationStatement.md) |
| PNet | TPM_00.50_RESPONSE | Patient profile mailing request response | [MedicationStatement](MedicationStatement.md) |
| PNet | TDU_00.50 | Drug utilization evaluation (DUE) information inquiry | [Medication](#interactions) |
| PNet | TDU_00.50_RESPONSE | Drug utilization evaluation (DUE) information inquiry response | [Medication](#interactions) |
| PNet | TDUTRP_00.50 | DUE inquiry with patient profile information (complete) | [MedicationStatement](MedicationStatement.md) |
| PNet | TDUTRP_00.50_RESPONSE | DUE inquiry with patient profile information (complete) response | [MedicationStatement](MedicationStatement.md) |
| PNet | TDUTRR_00.50 | DUE inquiry with patient profile information (latest) | [MedicationStatement](MedicationStatement.md) |
| PNet | TDUTRR_00.50_RESPONSE | DUE inquiry with patient profile information (latest) response | [MedicationStatement](MedicationStatement.md) |
| PNet | TRX_X1.X6 | Record prescription | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X1.X6_RESPONSE | Record prescription response | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X2.X7 | Change prescription status | [MedicationRequest](MedicationRequest.md) |
| PNet | TRX_X2.X7_RESPONSE | Change prescription status response | [MedicationRequest](MedicationRequest.md) |
| PNet | TMU_01.51 | Update medication | TBD |
| PNet | TMU_01.51_RESPONSE | Update medication response | TBD |
| PNet | TMU_11.61 | Update medication reversal | TBD |
| PNet | TMU_11.61_RESPONSE | Update medication reversal response | TBD |
| PNet | TPI_00.50 | Patient profile information update | [MedicationStatement](MedicationStatement.md) |
| PNet | TPI_00.50_RESPONSE | Patient profile information update response | [MedicationStatement](MedicationStatement.md) |

## PharmaNet Pharmacy Point of Service System Interactions

| Domain | Interaction Message | Description | FHIR Resource |
| ----- | ----- | ----- | ----- |


## Additional Domains: HL7-v3 Interactions

The following interactions are also needed by the private practice (e.g. EMR) profile. These interactions are not included in this API set, and may require differing integration requirements including system authorizations.

| Domain | Interaction Message | Description | FHIR Resource |
| ----- | ----- | ----- | ----- |
| CR | HCIM_IN_FindCandidates | Find Candidates | Patient |
| CR | HCIM_IN_FindCandidatesResponse | Find Candidates Response | Patient |
| CR | HCIM_IN_GetDemographics | Get Client Demographics | Patient |
| CR | HCIM_IN_GetDemographicsResponse | Get Client Demographics Response | Patient |
| CR | HCIM_IN_PersonRevised | Revise Person | Patient |
| CR | HCIM_IN_PersonRevisedResult | Revise Person Result | Patient |
| PLR | PRPM_IN306010 | Provider Details Query | Practitioner |
| PLR | PRPM_IN306011UV01 | Provider Details Query Response | Practitioner |
| PLR | PRPM_IN303010 | Update Provider Request | Practitioner |
| PLR | PRPM_IN303011 | Provider update response | Practitioner |

### Domain Legend

* PNet = PharmaNet
* CR = Healthcare Client Registry
* PLR = Provider Location Registry
