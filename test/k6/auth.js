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

import { post } from "k6/http";
import { Rate } from "k6/metrics";

// rates provide additional statistics in the end-of-test summary
const authenticationSuccessful = new Rate("_authentication_successful");

// tries to ensure that the client is authenticated by refreshing the current
// token or requesting a new one
export function authenticateClient(client, tokenUrl) {
    // there is no need to refresh the token if there are at least 45 seconds
    // until it expires
    if (client.token !== null && client.expires >= Date.now() + 45000) {
        return 200;
    }

    // request a new token if one doesn't exist already
    if (client.token === null || client.refresh === null) {
        return replaceClientToken(client, tokenUrl);
    }

    // otherwise, simply refresh the existing token
    return refreshClientToken(client, tokenUrl);
}

// submit the form data for a token refresh
function refreshClientToken(client, tokenUrl) {
    let formData = {
        "grant_type": "refresh_token",
        "client_id": client.id,
        "refresh_token": client.refresh,
    };

    // request a new token using the refresh form data
    let responseStatus = requestToken(client, tokenUrl, formData);

    // if the token refresh failed, request a new token instead
    if (responseStatus !== 200) {
        return replaceClientToken(client, tokenUrl);
    }

    return responseStatus;
}

// submit the form data for a new token
function replaceClientToken(client, tokenUrl) {
    let formData = {
        "grant_type": "client_credentials",
        "client_id": client.id,
        "audience": "pharmanet",
        "scope": client.scopes,
        "client_secret": client.secret
    };

    return requestToken(client, tokenUrl, formData);
}

// request a new client token using the given form data
function requestToken(client, tokenUrl, formData) {
    let response = post(tokenUrl, formData);
    let body = JSON.parse(response.body);

    if (response.status === 200) {
        // update the rate
        authenticationSuccessful.add(1);
        console.log("Client authentication successful");

        // update the client object
        client.token = body.access_token;
        client.refresh = body.refresh_token;
        client.expires = Date.now() + 1000 * body.expires_in;
    }
    else {
        // update the rate
        authenticationSuccessful.add(0);
        console.log("Client authentication not successful: " + response.status);

        console.log(
            "client.id=\"" + client.id + "\", " +
            "client.secret=\"" + client.secret + "\", " +
            "client.scopes=\"" + client.scopes + "\""
        );

        console.log("error\"" + body.error + ": " + body.error_description + "\"");
    }

    return response.status;
}
