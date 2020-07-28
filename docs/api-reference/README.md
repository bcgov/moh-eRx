# PharmaNet eRX REST API v1 Reference

Welcome to the PharmaNet REST API reference.

## Common Use Cases

## PharmaNet Private Practice Interactions for ePrescribing {#interactions}

| Domain | Interaction Message | Description | Resource Type |
| ----- | ----- | ----- | ----- |
| PNet | TIL_00.50 | Location inquiry | TBD |
| PNet | TIL_00.50_RESPONSE | Location inquiry response | TBD |
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
| PNet | TDUTRP_00.50 | DUE inquiry with patient profile information (complete) | [Medication](#interactions) |
| PNet | TDUTRP_00.50_RESPONSE | DUE inquiry with patient profile information (complete) response | [Medication](#interactions) |
| PNet | TDUTRR_00.50 | DUE inquiry with patient profile information (latest) | TBD |
| PNet | TDUTRR_00.50_RESPONSE | DUE inquiry with patient profile information (latest) response | TBD |
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

> PNet ::= PharmaNet

## Additional Domains: HL7v3 Interactions

| Domain | Interaction Message | Description | Resource Type |
| ----- | ----- | ----- | ----- |
| CR | HCIM_IN_FindCandidates | Find Candidates | Patient |
| CR | HCIM_IN_FindCandidatesResponse | Find Candidates Response | Patient |
| CR | HCIM_IN_GetDemographics | Get Client Demographics | Patient |
| CR | HCIM_IN_GetDemographicsResponse | Get Client Demographics Response | Patient |
| CR | HCIM_IN_PersonRevised | Revise Person | Patient |
| CR | HCIM_IN_PersonRevisedResult | Revise Person Result | Patient |

HIAL
MCCI_IN000002
Accept Ack
PR
PRPM_IN306010
Provider Details Query 
PR
PRPM_IN306011UV01
Provider Details Query Response 
PR
PRPM_IN303010
Update Provider Request
PR
PRPM_IN303011
Provider update response
HIAL
MCCI_IN000002UV01
Accept Ack
PLIS
POLB_IN374000CA
Lab Result Summary Query Request
PLIS
POLB_IN384000CA
Lab Result Summary Query Response
PLIS
POLB_IN354000CA
Request Query Results
PLIS
POLB_IN364000CA
Results Query Response
HIAL
MCCI_IN000002CA
Accept Ack
 


## Resources

- [MedicationRequest](MedicationRequest.md)
- [MedicationStatement](MedicationStatement.md)
