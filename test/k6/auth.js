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

// tries to ensure that the client is authenticated by requesting a new token if
// the current token is expired or expires within 45 seconds
export function authenticateClient(client, tokenUrl) {
    // there is no need to refresh the token if there are at least 45 seconds
    // until it expires
    if (client.token !== null && client.expires >= Date.now() + 45000) {
        return 200;
    }

    // assemble the form data for the authentication request
    let formData = {
        "grant_type": "client_credentials",
        "client_id": client.id,
        "audience": "pharmanet",
        "scope": client.scopes,
        "client_secret": client.secret
    };

    // submit the request and receive the response
    let response = post(tokenUrl, formData);
    let body = JSON.parse(response.body);

    if (response.status === 200) {
        // update the rate
        authenticationSuccessful.add(1);
        console.log("Client authentication successful");

        // update the client object
        client.token = body.access_token;
        client.expires = Date.now() + 1000 * body.expires_in;
    }
    else {
        // update the rate
        authenticationSuccessful.add(0);
        console.log("Client authentication not successful: " + response.status);

        console.log("error='" + body.error + ": " + body.error_description + "'");

        console.log(
            "client.id=" + client.id + ", " +
            "client.secret=" + client.secret + ", " +
            "client.scopes='" + client.scopes + "'"
        );
    }

    return response.status;
}
