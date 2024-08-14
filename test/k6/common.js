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

// environment and service are defined by environment variables set when the k6
// command to execute the test is run
const environment = __ENV.ERX_ENV;
const service = __ENV.ERX_SERVICE;
const iterationLength = __ENV.ERX_ITERATION_LENGTH ? __ENV.ERX_ITERATION_LENGTH : 1;

// the services follow a common naming scheme by environment, except prd which
// uses the plain url
const baseUrl = environment === "prd" ? "https://pnet.api.gov.bc.ca/api/v1/" : "https://pnet-" + environment + ".api.gov.bc.ca/api/v1/";

const serviceUrl = baseUrl + service;

// the keycloak url that will return the access token depends on the environment
let tokenUrl = "";

if (environment === "prd") {
    tokenUrl = "https://common-logon.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";
} else if (environment === "dev") {
    tokenUrl = "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token";
} else {
    tokenUrl = "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";
}

// each service requires different scopes from its users
// the test framework uses these with its token requests to keycloak
const scopes = {
    "Claim": "openid system/Claim.write system/Claim.read",
    "Consent": "openid system/Patient.read system/Consent.write system/Consent.read",
    "Location": "openid system/Location.read",
    "Medication": "openid system/Medication.read",
    "MedicationDispense": "openid system/MedicationDispense.write system/MedicationDispense.read",
    "MedicationRequest": "openid system/MedicationRequest.write system/MedicationRequest.read",
    "MedicationStatement": "openid system/MedicationStatement.read",
    "Patient": "openid system/Patient.read system/Patient.write",
    "Practitioner": "openid system/Practitioner.read"
};

// the client object stores information about the simulated client
const client = {
    id: __ENV.ERX_CLIENT,
    secret: __ENV.ERX_CLIENT_SECRET,
    token: null,
    expires: null,
    scopes: scopes[service],
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
