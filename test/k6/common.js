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

import { check, fail, sleep } from "k6";
import { post } from "k6/http";
import { b64decode, b64encode } from "k6/encoding";
import { Rate } from "k6/metrics";

import { v4 } from "./uuid.js";
import { examples } from "./examples.js";

// these are the options that are read by the k6 test framework
// here you can define the vus, iterations, duration, or stages
// see https://k6.io/docs/using-k6/k6-options/ for more information
export const options = {
    vus: __ENV.ERX_VUS ? __ENV.ERX_VUS : 1,
    iterations: __ENV.ERX_ITERATIONS ? __ENV.ERX_ITERATIONS : 1
};

// export const options = {
//     stages: [
//         {duration: "10s", target: 3},
//         {duration: "10s", target: 8},
//         {duration: "10s", target: 0},
//     ]
// };

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

// these Rates provide additional statistics in the end-of-test summary
const authSuccessRate = new Rate("authentication_successful");
const errorRate = new Rate("errors");
const refreshTokenSuccessRate = new Rate("auth_refresh_successful");

// this is the function k6 runs for each vu
// one execution of this function is one iteration
export default function() {
    let client = {
        clientId: __ENV.ERX_CLIENT,
        clientSecret: __ENV.ERX_CLIENT_SECRET,
        token: null,
        refresh: null,
        expires: null,
        scopes: scopes[service]
    };

    // run all test cases
    if (iterationLength == -1) {
        examples.forEach(transaction => {
            authorizeClient(client, tokenUrl);

            submitMessage(client, baseUrl + service, transaction);

            sleep(1);
        });
    }

    // choose a random test message from the list
    for (let i = 0; i < iterationLength; i++) {
        authorizeClient(client, tokenUrl);

        let transaction = examples[Math.floor(Math.random() * examples.length)];
        submitMessage(client, baseUrl + service, transaction);

        sleep(1);
    }
}

// tries to ensure that the client is authorized
// requests a new JWT or refreshes the current token if necessary
function authorizeClient(client, tokenUrl) {
    if ((__ITER == 0) && (client.token == null)) {
        let responseCode = authenticateClient(client, tokenUrl);

        if (!check(responseCode, {"Authenticated successfully": responseCode === 200})) {
            fail("Authentication failed with response code " + responseCode);
        }
    }

    refreshTokenIfNeeded(client, tokenUrl);
}

// helper function to provide the client with a token
function authenticateClient(client, tokenUrl) {
    // assemble the data to be passed to the keycloak endpoint
    let authFormData = {
        "grant_type": "client_credentials",
        "client_id": client.clientId,
        "audience": "pharmanet",
        "scope": client.scopes,
        "client_secret": client.clientSecret
    };

    // submit the request and receive the response
    let response = post(tokenUrl, authFormData);
    let jsonResponse = JSON.parse(response.body);

    if (response.status == 200) {
        // load the data into the client object
        client.token = jsonResponse["access_token"];
        client.refresh = jsonResponse["refresh_token"];
        client.expires = getAbsoluteTime(jsonResponse["expires_in"]);

        // log and record the success
        authSuccessRate.add(1);
        console.log("Authenticated client: " + client.clientId);
        console.log("Token: " + client.token);
    }
    else {
        console.log("Authentication failed with response code " + response.status + ".");
        console.log(
            "clientId=\"" + client.clientId + "\", " +
            "clientSecret=\"" + client.clientSecret + "\", " +
            "scopes=\"" + client.scopes + "\", " +
            "error=\"" + jsonResponse["error"] + ": " + jsonResponse["error_description"] + "\""
        );

        authSuccessRate.add(0);
        client.token = null;
    }

    return response.status;
}

// check if the token expires soon and refresh it if necessary
// refresh 45 seconds before expiry
function refreshTokenIfNeeded(client, tokenUrl) {
    if ((client.refresh != null) && (client.expires < (Date.now() + 45000))) {
        return refreshClient(client, tokenUrl);
    }
}

// refresh the token by requesting a new JET from keycloak
function refreshClient(client, tokenUrl) {
    // means our previous refresh failed
    if (client.token == null) {
        return authenticateClient(client);
    }

    let refreshFormData = {
        "grant_type": "refresh_token",
        "client_id": client.clientId,
        "refresh_token": client.refresh,
    };

    let response = post(tokenUrl, refreshFormData);
    let jsonResponse = JSON.parse(response.body);

    if (response.status == 200) {
        // load the data into the client object
        client.token = jsonResponse["access_token"];
        client.refresh = jsonResponse["refresh_token"];
        client.expires = getAbsoluteTime(jsonResponse["expires_in"]);

        // log and record the success
        refreshTokenSuccessRate.add(1);
        console.log("Re-authenticated client: " + client.clientId);
        console.log("Token: " + client.token);
    }
    else {
        console.log("Re-authentication failed with response code " + response.status + ". ");
        console.log(
            "clientId=\"" + client.clientId + "\", " +
            "clientSecret=\"" + client.clientSecret + "\", " +
            "scopes=\"" + client.scopes + "\", " +
            "error=\"" + jsonResponse["error"] + ": " + jsonResponse["error_description"] + "\""
        );

        refreshTokenSuccessRate.add(0);
        client.token = null;

        console.log("Attempting authentication with new token.")

        // pause before trying to authenticate the client with a new token
        sleep(1);
        return authenticateClient(client);
    }

    return response.status;
}

// send a transaction to the appropriate service in the appropriate environment
function submitMessage(client, url, transaction) {
    console.log("Request: " + transaction.name + " [" + transaction.version + "] " + transaction.purpose);

    // process and encode the message
    let payload = transaction.message.replace("${{ timestamp }}", getTimestamp());
    payload = b64encode(payload, "std");

    let timestamp = (new Date(Date.now())).toISOString().replace("Z", "").substring(0, 19) + "+00:00";

    // assemble the fhir-formatted JSON payload
    let fhirPayload = {
        "resourceType": "DocumentReference",
        "status": "current",
        "date": timestamp,
        "content": [
            {
                "attachment": {
                    "contentType": "x-application/hl7-v2+er7",
                    "data": payload
                }
            }
        ]
    };

    // optionally include a uuid with the message
    // the app should work whether or not this is present
    if (transaction.includeIdentifier) {
        let msgId = "urn:uuid:" + v4();

        fhirPayload["masterIdentifier"] = {
            "system": "urn:ietf:rfc:3986",
            "value": msgId
        };

        console.log("Generated identifier: " + msgId);
    }

    let params = {
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + client.token,
        },
    };

    console.log("Payload: " + payload);
    console.log("POST " + url);

    // post the message and receive the response
    let response = post(url, JSON.stringify(fhirPayload), params);
    let jsonResponse = JSON.parse(response.body);

    if (response.status == 200) {
        console.log("Transaction success.")
        console.log("HL7v2 Response: " + b64decode(jsonResponse.content[0].attachment.data, "std", "s"));
        errorRate.add(0);
    }
    else {
        console.log("Transaction failure with response code " + response.status + ".")
        console.log("Response body: " + response.body);
        errorRate.add(1);
    }
    return response;
}

// return an absolute time given the number of seconds until then
function getAbsoluteTime(seconds) {
    return Date.now() + seconds * 1000;
}

function getTimestamp() {
    let now = new Date(Date.now());
    let mon = now.getMonth() + 1;
    mon = (mon < 10) ? "0" + mon : mon;
    let day = now.getDate();
    day = (day < 10) ? "0" + day : day
    let str = now.getFullYear() + "/" +
        mon + "/" + day + " " +
        now.toLocaleTimeString("en-CA");
    return str;
}
