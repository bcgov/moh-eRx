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
import http from 'k6/http';
import { b64decode, b64encode } from 'k6/encoding';
import { check, fail, group, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';
import * as uuid from './uuid.js';

let virtualUsers = __ENV.ERX_VUS ? __ENV.ERX_VUS : 1;
let nIterations = __ENV.ERX_ITERATIONS ? __ENV.ERX_ITERATIONS : 1;

export let options = {
    vus: virtualUsers,
    iterations: nIterations
};

export let client_secret = __ENV.ERX_CLIENT_SECRET;
export let ClientId = __ENV.ERX_CLIENT ? __ENV.ERX_CLIENT : 'erx_development';

export let authSuccess = new Rate('authentication_successful');
export let errorRate = new Rate('errors');

export let refreshTokenSuccess = new Rate('auth_refresh_successful');

export let environment = (__ENV.ERX_ENV) ? __ENV.ERX_ENV : 'dev'; // default to test environment

export let TokenEndpointUrl_Dev = "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token";
export let TokenEndpointUrl_Test = "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";

export let baseUrl = "https://pnet-" + environment + ".api.gov.bc.ca/api/v1/";

export let ClaimServiceUrl = baseUrl + "Claim";
export let ConsentServiceUrl = baseUrl + "Consent";
export let LocationServiceUrl = baseUrl + "Location";
export let MedicationDispenseServiceUrl = baseUrl + "MedicationDispense";
export let MedicationRequestServiceUrl = baseUrl + "MedicationRequest";
export let MedicationServiceUrl = baseUrl + "Medication";
export let MedicationStatementServiceUrl = baseUrl + "MedicationStatement";
export let PatientServiceUrl = baseUrl + "Patient";
export let PractitionerServiceUrl = baseUrl + "Practitioner";


export let client =
{
    client_id: ClientId,
    client_secret: __ENV.ERX_CLIENT_SECRET,
    token: null,
    refresh: null,
    expires: null
};

function parseJwt(jwt) {
    var accessToken = jwt.split('.')[1];

    var decoded = b64decode(accessToken, "rawurl");
    var token_json = JSON.parse(decoded);
    return token_json;
};

export function getExpiresTime(seconds) {
    return (Date.now() + seconds * 1000);
}

export function authorizeClient(scopes) {
    if ((__ITER == 0) && (client.token == null)) {
        let loginRes = authenticateClient(client, scopes);
        if (!check(loginRes, {
            'Authenticated successfully': loginRes === 200
        })) {
            fail("Authentication Failed. Result is *not* 200 OK");
        }
    }
    refreshTokenIfNeeded(client);
    return client;
}

export function authenticateClient(client, scopes) {
    let auth_form_data = {
        grant_type: "client_credentials",
        client_id: client.client_id,
        audience: "pharmanet",
        scope: "openid " + scopes,
        client_secret: client.client_secret
    };

    var tokenUrl = TokenEndpointUrl_Dev;

    switch (environment) {
        case 'dev':
            tokenUrl = TokenEndpointUrl_Dev;
            break;
        case 'vs1':
        case 'vs2':
        case 'vc1':
        case 'vc2':
        case 'trn':
            tokenUrl = TokenEndpointUrl_Test;
            break;
        default:
            console.log("WARNING: \"" + environment + "\" is not a recognized environment. Defaulting to erx_development client ID.");
            tokenUrl = TokenEndpointUrl_Dev;
            break;
    }

    var res = http.post(tokenUrl, auth_form_data);
    var res_json = JSON.parse(res.body);

    if (res.status == 200) {
        client.token = res_json["access_token"];
        client.refresh = res_json["refresh_token"];
        var seconds = res_json["expires_in"];
        client.expires = getExpiresTime(seconds);
        authSuccess.add(1);
        console.log("Authenticated client: " + auth_form_data.client_id);
        console.log("Token: " + client.token);
    }
    else {
        console.log("Authentication Error for client=" + client.client_id +
            ", client_secret='" + client.client_secret + "'" +
            ", scope='" + scopes +
            "', ResponseCode='" + res.status +
            "', error='" + res_json.error +
            ": " + res_json.error_description + "'");
        authSuccess.add(0);
        client.token = null;
    }

    return res.status;
}

function refreshTokenIfNeeded(client) {

    if ((client.refresh != null) && (client.expires < (Date.now() + 45000))) // refresh 45 seconds before expiry
    {
        refreshClient(client);
    }
}

export function refreshClient(client) {

    if (client.token == null) {
        // means our previous refresh failed.
        return authenticateClient(client);
    }

    let refresh_form_data = {
        grant_type: "refresh_token",
        client_id: ClientId,
        refresh_token: client.refresh,
    };

    console.log("Getting Refresh Token for client: " + client.client_id);
    let res = http.post(TokenEndpointUrl, refresh_form_data);

    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        client.token = res_json["access_token"];
        client.refresh = res_json["refresh_token"];
        var seconds = res_json["expires_in"];
        client.expires = getExpiresTime(seconds);
        refreshTokenSuccess.add(1);
    }
    else {
        console.log("Token Refresh Error, client= " + client.client_id + ". ResponseCode[" + res.status + "] " + res.error);
        refreshTokenSuccess.add(0);
        client.token = null; // clear out the expiring token, forcing to re-authenticate.
        sleep(1);
        return authenticateClient(client);
    }
    return res.status;
}

export function params(client) {
    var params = {
        headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + client.token,
        },
    };
    return params;
}

export function postMessage(url, payload, includeIdentifier = true) {
    var now = new Date(Date.now());

    var msgId = "urn:uuid:" + uuid.v4();

    var timestamp = now.toISOString().replace("Z", "");
    var timestamp = timestamp.substring(0, 19) + "+00:00";

    var fhirPayload = {
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

    if (includeIdentifier) {
        fhirPayload["masterIdentifier"] = {
            "system": "urn:ietf:rfc:3986",
            "value": msgId
        };
        console.log("Generated identifier: " + msgId);
    }

    console.log("Payload:= " + payload);
    console.log("[ERX_ENV= " + environment + "] POST " + url);


    var res = http.post(url, JSON.stringify(fhirPayload), params(client));
    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        //console.log(JSON.stringify(res_json));
        console.log("HL7v2 Response = " + b64decode(res_json.content[0].attachment.data, "std", "s"));
        errorRate.add(0);
    }
    else {
        console.log("[ResponseCode= " + res.status + "]");
        console.log(res.body);
        errorRate.add(1);
    }
    return res;
}

export function submitMessage(url, example) {
    console.log("Request: " + example.name + ' [' + example.version + '] ' + example.purpose);
    var payload = Hl7v2Encoded(example.message); // Returns Base64 encoded hl7v2 message
    return postMessage(url, payload, example.includeIdentifier);
}

export function submitHL7MessageBase64(url, b64Payload) {
    return postMessage(url, b64Payload, true);
}

function encode(hl7Message) {
    console.log(hl7Message);
    return b64encode(hl7Message, 'std');
}

function Hl7v2Encoded(message) {
    var res = message.replace("${{ timestamp }}", timestamp());
    return encode(res);
}

function timestamp() {
    var now = new Date(Date.now());
    var mon = now.getMonth() + 1;
    mon = (mon < 10) ? "0" + mon : mon;
    var day = now.getDate();
    day = (day < 10) ? "0" + day : day
    var str = now.getFullYear() + "/" +
        mon + "/" + day + " " +
        now.toLocaleTimeString("en-CA");
    return str;
}
