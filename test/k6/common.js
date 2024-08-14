//-------------------------------------------------------------------------
// Copyright © 2021 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------

import { sleep } from "k6";

import { authenticateClient } from "./auth.js";
import { submitMessage } from "./transaction.js";
import { examples } from "./examples.js";
import { meanDelaySeconds } from "./options.js";

export { options } from "./options.js";

// dictionary to select the correct base api url by environment
const environmentBaseApiUrls = {
    "dev": "https://pnet-dev.api.gov.bc.ca/api/v1/",
    "vs1": "https://pnet-vs1.api.gov.bc.ca/api/v1/",
    "tr1": "https://pnet-tr1.api.gov.bc.ca/api/v1/",
    "vc2": "https://pnet-vc2.api.gov.bc.ca/api/v1/",
    "vc1": "https://pnet-vc1.api.gov.bc.ca/api/v1/",
    "prd": "https://pnet.api.gov.bc.ca/api/v1/",
};

// dictionary to select the correct keycloak token url by environment
const environmentTokenUrls = {
    "dev": "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token",
    "vs1": "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token",
    "tr1": "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token",
    "vc2": "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token",
    "vc1": "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token",
    "prd": "https://common-logon.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token",
};

// dictionary to capitalize the service name by service
const serviceNames = {
    "claim": "Claim",
    "consent": "Consent",
    "location": "Location",
    "medication": "Medication",
    "medicationdispense": "MedicationDispense",
    "medicationrequest": "MedicationRequest",
    "medicationstatement": "MedicationStatement",
    "patient": "Patient",
    "practitioner": "Practitioner",
};

// dictionary to select the keycloak scopes by service
const serviceScopes = {
    "claim": "openid system/Claim.write system/Claim.read",
    "consent": "openid system/Patient.read system/Consent.write system/Consent.read",
    "location": "openid system/Location.read",
    "medication": "openid system/Medication.read",
    "medicationdispense": "openid system/MedicationDispense.write system/MedicationDispense.read",
    "medicationrequest": "openid system/MedicationRequest.write system/MedicationRequest.read",
    "medicationstatement": "openid system/MedicationStatement.read",
    "patient": "openid system/Patient.read system/Patient.write",
    "practitioner": "openid system/Practitioner.read",
};

// environment and service are defined by environment variables set when the k6
// command to execute the test is run
const environment = __ENV.ERX_ENV;
const service = __ENV.ERX_SERVICE;
const iterationLength = __ENV.ERX_ITERATION_LENGTH ? __ENV.ERX_ITERATION_LENGTH : 1;

const serviceUrl = environmentBaseApiUrls[environment] + serviceNames[service];
const tokenUrl = environmentTokenUrls[environment];

// the client object stores information about the simulated client
const client = {
    id: __ENV.ERX_CLIENT,
    secret: __ENV.ERX_CLIENT_SECRET,
    token: null,
    expires: null,
    scopes: serviceScopes[service],
};

// k6 runs this function once for each iteration
export default function() {
    // choose a random test message from the list for each transaction
    for (let i = 0; i < iterationLength; i++) {
        authenticateClient(client, tokenUrl);

        let transaction = examples[Math.floor(Math.random() * examples.length)];

        submitMessage(client, serviceUrl, transaction);

        sleep(randomExp(meanDelaySeconds));
    }

    // when the user sets iterationLength to -1, run all available transactions
    if (iterationLength == -1) {
        examples.forEach(transaction => {
            authenticateClient(client, tokenUrl);

            submitMessage(client, serviceUrl, transaction);

            sleep(randomExp(meanDelaySeconds));
        });
    }
}

// generate a random number, exponentially distributed with the given mean
function randomExp(mean) {
    return Math.log(1 - Math.random()) * -mean;
}
