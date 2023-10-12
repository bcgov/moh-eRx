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
import { Rate } from "k6/metrics";

// Rates provide additional statistics in the end-of-test summary
const authSuccessRate = new Rate("authentication_successful");
const refreshTokenSuccessRate = new Rate("auth_refresh_successful");

// tries to ensure that the client is authorized
// requests a new JWT or refreshes the current token if necessary
export function authorizeClient(client, tokenUrl) {
    if ((__ITER == 0) && (client.token == null)) {
        let responseCode = authenticateClient(client, tokenUrl);

        if (!check(responseCode, {"Authentication successful": responseCode === 200})) {
            fail("Authentication failed with response code " + responseCode);
        }
    }

    refreshClientToken(client, tokenUrl);
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
        console.log("Authentication failed with response code " + response.status);
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
// refresh the token by requesting a new JWT from keycloak
function refreshClientToken(client, tokenUrl) {
    if ((client.refresh == null) || (client.expires >= (Date.now() + 45000))) {
        // don't need to refresh
        return;
    }

    if (client.token == null) {
        // previous refresh failed
        return authenticateClient(client, tokenUrl);
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
        return authenticateClient(client, tokenUrl);
    }

    return response.status;
}

// return an absolute time given the number of seconds until then
function getAbsoluteTime(seconds) {
    return Date.now() + seconds * 1000;
}
