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
import { b64decode } from 'k6/encoding';
import { check, group, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';
import * as uuid from './uuid.js';



export let client_secret = __ENV.ERX_CLIENT_SECRET;
export let client_id = __ENV.ERX_CLIENT;

export let maxVus = (__ENV.VUS) ? __ENV.VUS : 300; 
maxVus = (maxVus < 1) ? 1 : maxVus;

export let authSuccess = new Rate('authentication_successful');
export let errorRate = new Rate('errors');

export let refreshTokenSuccess = new Rate('auth_refresh_successful');

export let environment = (__ENV.ERX_ENV) ? __ENV.ERX_ENV : 'dev'; // default to test environment

export let TokenEndpointUrl = "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token";

export let baseUrl = "https://" + environment + "-";
export let ClaimServiceUrl = baseUrl + "claimservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Claim";
export let ConsentServiceUrl = baseUrl + "consentservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Consent";
export let LocationServiceUrl = baseUrl + "locationservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Location";
export let MedicationDispenseServiceUrl = baseUrl + "medicationdispenseservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/MedicationDispense";
export let MedicationRequestServiceUrl = baseUrl + "medicationrequestservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/api/v1/MedicationRequest";
export let MedicationServiceUrl = baseUrl + "medicationservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Medication";
export let MedicationStatementService = baseUrl + "medicationstatementservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/MedicationStatement";
export let PatientService = baseUrl + "patientservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Patient";
export let PractitionerService = baseUrl + "practitionerservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Practitioner";

export let ClientId = __ENV.ERX_CLIENT ? __ENV.ERX_CLIENT : 'erx_development';

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
        check(loginRes, {
            'Authenticated successfully': loginRes === 200
        });
    }
    refreshTokenIfNeeded(client);
    return client;
}

export function authenticateClient(client, scopes) {
    let auth_form_data = {
        grant_type: "client_credentials",
        client_id: client.client_id,
        audience: "pharmanet",
        scope: scopes,
        client_secret: client.client_secret
    };
    console.log("Authenticating client: " + auth_form_data.client_id);
    var res = http.post(TokenEndpointUrl, auth_form_data);
    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        client.token = res_json["access_token"];
        client.refresh = res_json["refresh_token"];
        var seconds = res_json["expires_in"];
        client.expires = getExpiresTime(seconds);
        authSuccess.add(1);
    }
    else {
        console.log("Authentication Error for client= " + client.client_id + 
        "client_secret=" + client.client_secret + 
        ", scope=" + client.scope + 
        ", ResponseCode[" + res.status + 
        "] " + res.error);
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

export function postMessage(url, payload) {
    var now = new Date(Date.now());
    console.log("POST " + url);

    var params = this.params(common.client);

    var msgId = "urn:uuid:" + uuid.v4(); 

    var timestamp = now.toISOString().replace("Z", "");
    var timestamp = timestamp.substr(0, 19) + "+00:00";

    var fhirPayload = {
        "resourceType": "DocumentReference",
        "masterIdentifier": {
            "system": "urn:ietf:rfc:3986",
            "value": msgId
        },
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

    console.log(JSON.stringify(fhirPayload));

    var res = http.post(url, fhirPayload, params);
    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        console.log(JSON.stringify(res_json));
        errorRate.add(0);
    }
    else {
        console.log("[ResponseCode= " + res.status + "]");
        errorRate.add(1);
    }
    return res;
}
