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

import { authorizeClient } from "./auth.js";
import { submitMessage } from "./transaction.js";
import { examples } from "./examples.js";

// these are the options that are read by the k6 test framework
// here you can define the vus, iterations, duration, or stages
// see https://k6.io/docs/using-k6/k6-options/ for more information
export const options = {
    vus: __ENV.ERX_VUS ? __ENV.ERX_VUS : 1,
    iterations: __ENV.ERX_ITERATIONS ? __ENV.ERX_ITERATIONS : 1
};

// environment and service are defined by environment variables
// their values are set when the k6 command to execute the test is run
const environment = __ENV.ERX_ENV;
const service = __ENV.ERX_SERVICE;
const iterationLength = __ENV.ERX_ITERATION_LENGTH ? __ENV.ERX_ITERATION_LENGTH : 1;

// the services follow a common naming scheme by environment
// they look like https://pnet-{env}.api.gov.bc.ca/api/v1/{service}
// except prod, which is https://pnet.api.gov.bc.ca/api/v1/{service}
const baseUrl = environment == "prd" ?
    "https://pnet.api.gov.bc.ca/api/v1/" :
    "https://pnet-" + environment + ".api.gov.bc.ca/api/v1/";

const serviceUrl = baseUrl + service;

// the keycloak url that will return the access token
// the url to use depends on the environment
// the dev environment uses the dev url, the prd environment uses the prod url,
// and the other environments all use the test url
const tokenUrl = environment == "dev" ?
    "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token" :                // dev
    environment == "prd" ?
        "https://common-logon.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token" :      // prod
        "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";  // test

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
    clientId: __ENV.ERX_CLIENT,
    clientSecret: __ENV.ERX_CLIENT_SECRET,
    token: null,
    refresh: null,
    expires: null,
    scopes: scopes[service]
};

// this is the function k6 runs for each vu
// one execution of this function is one iteration
export default function() {
    // choose a random test message from the list for each transaction
    for (let i = 0; i < iterationLength; i++) {
        authorizeClient(client, tokenUrl);

        let transaction = examples[Math.floor(Math.random() * examples.length)];
        submitMessage(client, serviceUrl, transaction);

        sleep(1);
    }

    // run all test cases only when the user sets iterationLength to -1
    if (iterationLength == -1) {
        examples.forEach(transaction => {
            authorizeClient(client, tokenUrl);

            submitMessage(client, serviceUrl, transaction);

            sleep(1);
        });
    }
}
