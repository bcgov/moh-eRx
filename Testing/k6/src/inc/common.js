//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
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

export let client_secret = __ENV.ERX_CLIENT_SECRET;

export let maxVus = (__ENV.VUS) ? __ENV.VUS : 300; 
maxVus = (maxVus < 1) ? 1 : maxVus;
export let rampVus = (maxVus / 4).toFixed(0);
rampVus = (rampVus < 1) ? 1 : rampVus;

export let authSuccess = new Rate('authentication_successful');
export let errorRate = new Rate('errors');

export let refreshTokenSuccess = new Rate('auth_refresh_successful');

export let environment = (__ENV.ERX_ENV) ? __ENV.ERX_ENV : 'dev'; // default to test environment

export let smokeOptions = {
    vus: 5,
    iterations: 5,
  }

export let loadOptions = {
    vu: maxVus,
    stages: [
        { duration: '2m', target: rampVus }, // simulate ramp-up of traffic from 1 users over a few minutes.
        { duration: '3m', target: rampVus }, // stay at number of users for several minutes
        { duration: '3m', target: maxVus }, // ramp-up to users peak for some minutes (peak hour starts)
        { duration: '3m', target: maxVus }, // stay at users for short amount of time (peak hour)
        { duration: '2m', target: rampVus }, // ramp-down to lower users over 3 minutes (peak hour ends)
        { duration: '3m', target: rampVus }, // continue for additional time
        { duration: '2m', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        'errors': ['rate < 0.05'], // threshold on a custom metric
        'http_req_duration': ['p(90)< 9000'], // 90% of requests must complete this threshold 
        'http_req_duration': ['avg < 5000'], // average of requests must complete within this time
    },
}
export let groupDuration = Trend('batch');

export let baseUrl = "https://" + environment + "-";
export let TokenEndpointUrl = "https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos/protocol/openid-connect/token";
export let ClaimServiceUrl = baseUrl + "claimservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Claim";
export let ConsentServiceUrl = baseUrl + "consentservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Consent";
export let LocationServiceUrl = baseUrl + "locationservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Location";
export let MedicationDispenseServiceUrl = baseUrl + "medicationdispenseservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/MedicationDispense";
export let MedicationRequestServiceUrl = baseUrl + "medicationrequestservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/MedicationRequest";
export let MedicationService = baseUrl + "medicationservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Medication";
export let MedicationStatementService = baseUrl + "medicationstatementservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/MedicationStatement";
export let PatientService = baseUrl + "patientservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Patient";
export let PractitionerService = baseUrl + "practitionerservice-erx.apps.silver.devops.gov.bc.ca/Pharmanet/v1/api/Practitioner";

export let ClientId = (__ENV.HG_CLIENT) ? __ENV.CLIENT_ID : 'erx_development'; // default to k6 client id
export let OptionsType = (__ENV.TEST) ? __ENV.TEST : 'load'; // default test type

function parseJwt(jwt) {
    var accessToken = jwt.split('.')[1];

    var decoded = b64decode(accessToken, "rawurl");
    var token_json = JSON.parse(decoded);
    return token_json;
};


export function groupWithDurationMetric(name, group_function) {
    let start = new Date();
    group(name, group_function);
    let end = new Date();
    groupDuration.add(end - start, { groupName: name });
}

export function OptionConfig() {

    if (OptionsType == 'load') {
        return loadOptions;
    }
    if (OptionsType === 'smoke') {
        return smokeOptions;
    }
    return loadOptions;
}

export function getExpiresTime(seconds) {
    return (Date.now() + seconds * 1000);
}

export function authorizeClient(client) {
    if (((__ITER == 0) && (client.token == null)) {
        let loginRes = authenticateClient(client);
        check(loginRes, {
            'Authenticated successfully': loginRes === 200
        });
    }
    refreshTokenIfNeeded(client);
}

function authenticateClient(client) {

    let auth_form_data = {
        grant_type: "client_credentials_grant",
        client_id: ClientId,
        audience: "pharmanet",
        scope: "openid system/*.write system/*.read",
        client_secret: client.client_secret,
    };
    console.log("Authenticating client: " + auth_form_data.client_id);
    var res = http.post(TokenEndpointUrl, auth_form_data);
    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        client.token = res_json["access_token"];
        client.refresh = res_json["refresh_token"];
        var seconds = res_json["expires_in"];
        user.expires = getExpiresTime(seconds);
        authSuccess.add(1);
    }
    else {
        console.log("Authentication Error for client= " + client.client_id + ". ResponseCode[" + res.status + "] " + res.error);
        authSuccess.add(0);
        client.token = null;
    }

    return res.status;
}

function refreshTokenIfNeeded(user) {

    if ((user.refresh != null) && (user.expires < (Date.now() + 45000))) // refresh 45 seconds before expiry
    {
        refreshUser(user);
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

    console.log("Getting Refresh Token for username: " + client.client_id);
    let res = http.post(TokenEndpointUrl, refresh_form_data);

    if (res.status == 200) {
        var res_json = JSON.parse(res.body);
        user.token = res_json["access_token"];
        user.refresh = res_json["refresh_token"];
        var seconds = res_json["expires_in"];
        user.expires = getExpiresTime(seconds);
        refreshTokenSuccess.add(1);
    }
    else {
        console.log("Token Refresh Error for user= " + client.client_id + ". ResponseCode[" + res.status + "] " + res.error);
        refreshTokenSuccess.add(0);
        client.token = null; // clear out the expiring token, forcing to re-authenticate.
        sleep(1);
        return authenticateClient(client);
    }
    return res.status;
}

export function getRandomInteger(min, max) {
    return Math.floor(Math.random() * (max - min) + min);
}

export function getRandom(min, max) {
    return Math.random() * (max - min) + min;
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

export function MedicationDispense(client) {
    let medicationDispenseRequests = {
        'dispense': {
            method: 'POST',  
            url: common.CommentUrl,
            body: ""
        }
    };
    return medicationDispenseRequests;
}

function isObject(val) {
    if (val === null) { return false;}
    return ((typeof val === 'object') );
}

export function checkResponse(response) {
    if (isObject(response))
    {
        var ok = check(response, {
            "HttpStatusCode is 200": (r) => r.status === 200,
            "HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "HttpStatusCode is NOT 402 Payment Required": (r) => (r.status != 402),
            "HttpStatusCode is NOT 403 Forbidden": (r) => (r.status != 403),
            "HttpStatusCode is NOT 404 Not Found": (r) => (r.status != 404),
            "HttpStatusCode is NOT 405 Method Not Allowed": (r) => (r.status != 405),
            "HttpStatusCode is NOT 406 Not Acceptable": (r) => (r.status != 406),
            "HttpStatusCode is NOT 407 Proxy Authentication Required": (r) => (r.status != 407),
            "HttpStatusCode is NOT 408 Request Timeout": (r) => (r.status != 408),
            "HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 409) && (r.status <= 499)),
            "HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
        return; 
    }
}

export function checkResponses(responses) {
    if (responses['beta']) {
        var ok = check(responses['beta'], {
            "Beta HttpStatusCode is 200": (r) => r.status === 200,
            "Beta HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "Beta HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "Beta HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "Beta HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "Beta HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['comments']) {
        check(responses['comments'], {
            "Beta HttpStatusCode is 200": (r) => r.status === 200,
            "Beta HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "Beta HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "Beta HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "Beta HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "Beta HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['communication']) {
        check(responses['communication'], {
            "Communication HttpStatusCode is 200": (r) => r.status === 200,
            "Communication HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "Communication HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "Communication HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "Communication HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "Communication HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['configuration']) {
        check(responses['configuration'], {
            "Configuration HttpStatusCode is 200": (r) => r.status === 200,
            "Configuration HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "Configuration HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "Configuration HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "Configuration HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "Configuration HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['labs']) {
        check(responses['labs'], {
            "LaboratoryService HttpStatusCode is 200": (r) => r.status === 200,
            "LaboratoryService HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "LaboratoryService HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "LaboratoryService HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "LaboratoryService HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "LaboratoryService HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['meds']) {
        check(responses['meds'], {
            "MedicationService HttpStatusCode is 200": (r) => r.status === 200,
            "MedicationService HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "MedicationService HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "MedicationService HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "MedicationService HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "MedicationService HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['notes']) {
        check(responses['notes'], {
            "Note HttpStatusCode is 200": (r) => r.status === 200,
            "Note HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "Note HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "Note HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "Note HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "Note HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['patient']) {
        check(responses['patient'], {
            "PatientService HttpStatusCode is 200": (r) => r.status === 200,
            "PatientService HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "PatientService HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "PatientService HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "PatientService HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "PatientService HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
    if (responses['profile']) {
        check(responses['profile'], {
            "UserProfile HttpStatusCode is 200": (r) => r.status === 200,
            "UserProfile HttpStatusCode is NOT 3xx Redirection": (r) => !((r.status >= 300) && (r.status <= 306)),
            "UserProfile HttpStatusCode is NOT 401 Unauthorized": (r) => (r.status != 401),
            "UserProfile HttpStatusCode is NOT 4xx Client Error": (r) => !((r.status >= 400) && (r.status <= 499)),
            "UserProfile HttpStatusCode is NOT 5xx Server Error": (r) => !((r.status >= 500) && (r.status <= 598)),
            "UserProfile HttpStatusCode is NOT 0 (Timeout Error)": (r) => (r.status != 0),
        }) || errorRate.add(1);
    }
}
