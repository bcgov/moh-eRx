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
import { b64decode, b64encode } from "k6/encoding";
import { Rate } from "k6/metrics";

import { v4 } from "./uuid.js";

// rates provide additional statistics in the end-of-test summary
const transactionSuccessful = new Rate("_transaction_successful");

// send a transaction to the appropriate service in the appropriate environment
export function submitMessage(client, url, transaction) {
    console.log("Request: " + transaction.name + " [" + transaction.version + "] " + transaction.purpose);

    // process and encode the message
    let payload = transaction.message.replace("${{ timestamp }}", getTimestamp());
    payload = b64encode(payload, "std");

    let timestamp = (new Date(Date.now())).toISOString();

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
    if (transaction.includeIdentifier) {
        let msgId = "urn:uuid:" + v4();

        fhirPayload["masterIdentifier"] = {
            "system": "urn:ietf:rfc:3986",
            "value": msgId
        };

        console.log("Generated identifier: " + msgId);
    }

    // set request headers
    let params = {
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + client.token,
        },
    };

    console.log("Payload: " + payload);
    console.log("Endpoint: " + url);

    // submit the request and receive the response
    let response = post(url, JSON.stringify(fhirPayload), params);
    let body = JSON.parse(response.body);

    if (response.status === 200) {
        // update the rate
        transactionSuccessful.add(1);
        console.log("Transaction successful");

        console.log("HL7v2 Response: " + b64decode(body.content[0].attachment.data, "std", "s"));
    }
    else {
        // update the rate
        transactionSuccessful.add(0);
        console.log("Transaction not successful: " + response.status);

        if (body) {
            console.log("error='" + body.error + "'");
        }
    }

    return response.status;
}

// return a formatted timestamp for an HL7v2 timestamp/date field
function getTimestamp() {
    let now = new Date(Date.now());

    let mon = now.getMonth() + 1;
    mon = (mon < 10) ? "0" + mon : mon;

    let day = now.getDate();
    day = (day < 10) ? "0" + day : day

    let str = now.getFullYear() + "/" + mon + "/" + day + " " + now.toLocaleTimeString("en-CA");

    return str;
}
