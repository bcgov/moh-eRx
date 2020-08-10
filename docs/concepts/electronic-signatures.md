# Electronic Signatures

The College of Physicians and Surgeons of British Columbia require that all authorized prescribers supply a 'wet' electronic signature with each digital electronic prescription written. 

## When to Use by Prescribing Practitioners

An electronic signature, in the form of a digital image capture of a scribed or keyed in 'wet signature' is required for each prescription written. It must be a 'wet' signature, meaning that it is generated at the time the prescription is written.  

The PNet Transaction where this is supplied is the [Prescription Record Request](../api-reference/MedicationRequest.md) transaction, as submitted to the MedicationRequest microservice endpoint. 

## Returning the prescribing practitioner's signature

The MedicationRequest interactions that retrieve a prescription return the corresponding electronic signature in the HL7 FHIR Resource Bundle. The pharmacist is then able to view the electronic copy of the 'wet' signature.

In the future, the signature requirement may evolve to that of a digital signature format, such as [W3C XML Digital Signature](https://www.w3.org/Signature/Activity.html)

### Example

The example request below represents a Record Prescription interaction with PharmaNet by a prescriber's system. The mandatory electronic signature is supplied as a Portable Network Graphics image:

```code
  POST https://moh.api.gov.bc.ca/PharmaNet/v1/MedicationRequest/ HTTP/1.1
  Date: Sat, 30 Jul 2020 01:10:02 GMT
  Content-Length: 553
  Content-Type: application/fhir+json
  Authorization: "Bearer TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNl..."
  
    {
        "resourceType": "Bundle",
        "identifier": "9406ccdd-be4e-4a91-95a5-96e8cf33b53a",
        "type": "message",
        "timestamp": "2020-07030T01:10:01Z",
        "entry": [
            "resource": {
                "resourceType": "MessageHeader",
                "eventCoding": {
                    "system" : "http://api.example.org/PharmaNet/hl7-v2-transactions"
                    "code" : "TRX_X1.X6",
                },
                "source": {
                    "name": "YourEMR",
                    "endpoint": "https://www.your.emr.app"
                }
            },
            "resource": {
                "resourceType": "Binary",
                "contentType": "x-application/hl7-v2+er7",
                "data": "Fib3JpcyBuaXNpIHV0IGFsaXF1aXAgZXggZWEgY29tbW9yZSBkb2xvciBpbiB..."
            }
        ],
        "signature": {
            "type": "authorship",
            "when": "2020-07030T01:09:57Z",
            "sigFormat": "image/png",
            "data": "w6EKGkV4aWYAAE1NACoAAAAIAAsBDwACAAAABgAAAMKS..."
        }
    }
```

## For More Information

### College of Physicians and Surgeons of British Columbia

For more information about the requirement for 'wet' signatures, please see [College of Physicians and Surgeons of British Columbia](https://www.cpsbc.ca/for-physicians/college-connector/2014-V02-02/05)
  
The key message from CPSBC is that *the signature must be unique and applied with a human hand—be it pen to paper, or electronic pen to pad.*

### PODSA

- The [Pharmacy Operations and Drug Scheduling Act](http://library.bcpharmacists.org/6_Resources/6-1_Provincial_Legislation/5082-PODSA_Bylaws.pdf) (PODSA) Bylaws provide guidance regarding electronic signatures, with the following definition:

#### What is an acceptable form of electronic signature

**electronic signature** means:

(a) information in electronic form that a person has created or adopted in order to
sign a record, other than with respect to a prescription signed by a full pharmacist for the purpose of prescribing, that is in, attached to or associated with a record, is secure and is only reproducible and used by that person, and,

(b) with respect to a prescription signed by a full pharmacist for the purpose of prescribing, the electronic signature must meet the requirements of paragraph (a) and must be a unique mark personally applied by that pharmacist;

*From* 5082-PODSA_Bylaws v2020.3 Effective 2020-05-07 (Posted 2020-05-07) College of Pharmacists of BC – PODSA Bylaws

### College of Pharmacists of British Columbia

Further guidance from the [College of Pharmacists of British Columbia](https://bcpharmacists.org) elaborate on how uniqueness is achieved in the following article: [*On-Call - Accepting an electronic signature*](https://www.bcpharmacists.org/readlinks/call-accepting-electronic-prescription):

*Electronic prescriptions are only permitted if the electronic prescriber’s signature is unique. Health Canada considers a unique electronic signature to be equivalent to a paper and pen signature. Therefore the signature must be a fresh new signature written on the prescription with an electronic pen pad, similar to signing a pen and paper prescription. **Cutting and pasting a signature into an electronic prescription is not permitted.***

### BC Government electronic signature guidance

- For BC Government guidance on electronic signatures, see [Electronic Signatures](https://www2.gov.bc.ca/assets/gov/british-columbians-our-governments/services-policies-for-government/information-technology/electronic_signatures_guide.pdf)
