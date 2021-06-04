# K6 moh-eRx Non-Functional (Performance) Tests

k6 is a free and open-source load testing tool written in Go language with tests scripted in JavaScript.

## Virtual Users (vus)

k6 uses the concept of virtual users. When specifying the number of virtual users, they run concurrently over the script. Each iteration of the script represents a single user operating that script. So, if 10 vus ran for 1 minute and each iteration of the script took 10 seconds, then the total number of iterations run would be 10 x 60/10 = 60 iterations completed.

For more information on virtual users, see the k6 documentation: [what are virtual users](https://k6.io/docs/cloud/cloud-faq/what-are-vus-virtual-users).

### Calculation the number of VUS max

```code
VUs = (hourly sessions * average session duration in seconds)/3600
```

> For example, if we expect up to 10,000 transactions hitting the moh-eRx APIs within a given hour, and we expect that the average session duration is 1 minute, then the VUS would be 167.

See [k6 calculation guidance](https://k6.io/docs/cloud/cloud-faq/what-are-vus-virtual-users)

## Smoke Testing

The Smoke Test's role is to verify that your System can handle minimal load, without any problems. 
Our smoke tests uses by default 1 virtual user and executes the API call iteratively for 10 seconds duration to ensure it returns a 200 OK.

Any errors here are an indication of functionality not working under basic load.

The choices for ERX_ENV are 'dev' or 'vs1' (vendor staging 1)

```bash
export ERX_ENV=dev
export ERX_CLIENT_SECRET=<client_credentials_grant_secret> 
bash smoke.sh ./src/k6_MedicationRequest.js
```

### When to run the smoke test

Run this test often, after each system change/release.  This ensures that functionality has not broken under basic loads.

## Load Testing

Load testing is primarily concerned with assessing the systems performance, the purpose of stress testing is to assess the availability and stability of the system under heavy load. You can play with the VUS and Iterations within the shell script.

```bash
export ERX_ENV=dev
export ERX_CLIENT_SECRET=<client_credentials_grant_secret>
bash load.sh ./src/k6_Patient.js
```
Future: Add more sophisticated load test script that includes ramp up time and groups logical sequences of api calls mimicking real world flow.

## Stress Testing

Stress Testing is a type of load testing used to determine the limits of the system. The purpose of this test is to verify the stability and reliability of the system under extreme conditions. Set the vus in the load test to be really high.

## Sample HL7v2 Messages from Pharmanet

The web service is deployed to DEV and it is ready for your team to submit the transaction. The following sample requests work in DEV.

```bash
REQUEST :
MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|||ZPN^^|3362|P|2.1||ZZZ|TID||3362|P1|6H2O2||ZCA||03|00|KC|13ZCB|BC00007007|200916|3362|ZCC||||||||||0009433498542|

RESPONSE :
   MSH|^~\&|TRXTOOL|PCARESUP|TRXTOOL|PCARESUP|||ZPN|003362|P|2.1|
   ZCB|BC00007007|200916|3362
   ZZZ|TID|0|3362|P1|6H2O2||0 Operation successful|
   ZCC|||||19450705|||||0009433498542|F
   ZPA|FYGZC|KJUON|W|ZPA1^^^604^1599209|ZPA2^M^^^^^WVROHFSVCSIX^^SURREY^CAN^V4A3B0^^BC^^^^^^^^^
 
 
REQUEST :
MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|||ZPN^^|3365|P|2.1||ZZZ|TRP||3365|P1|3E9V1|||PHSVE105|ZCA||03|00|KC|13|ZCB|BC00007007|200916|3365|ZCC||||||||||0009388880284|

RESPONSE :
   MSH|^~\&|TRXTOOL|PCARESUP|TRXTOOL|PCARESUP|||ZPN|003365|P|2.1
   ZCB|BC00007007|200916|3365
  ZZZ|TRP|0|3365|P1|3E9V1||0 Operation successful|PHSVE105
   ZCC||||||||||0009388880284
   ZPB|
 
 
REQUEST :
MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|||ZPN^^|3371|P|2.1||
ZZZ|TRS||3371|P1|1D5T2|||RAHIMAN|
ZCA||03|00|KC|13|ZCB|BC00007007|200916|3371|
ZCC||||||||||0009427405543|

RESPONSE :
   MSH|^~\&|TRXTOOL|PCARESUP|TRXTOOL|PCARESUP|||ZPN|003371|P|2.1
   ZCB|BC00007007|200916|3371
   ZZZ|TRS|0|3371|P1|1D5T2||3049 Operation Successful: Rx's not filled here.|RAHIMAN
   ZCC||||||||||0009427405543
   ZPB|
 ```

