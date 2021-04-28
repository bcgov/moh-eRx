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
import { b64encode } from 'k6/encoding';

export let MedicationRequest_ZPN_TRX_X0 = "MSH|^~\&|DESKTOP|EMR|DESKTOP|EMRMD|{{ timestamp }}|userID:192.168.0.1|ZPN|691365|P|2.1||" + "\n" +
"ZCA||70|X0|MA|01|" + "\n" +
"ZCB|LDJQQ|140827|691365"  + "\n" +
"ZCC||||||||||000nnnnnnnnnn|"  + "\n" +
"ZPR|||||||||||"  + "\n" +
"ZZZ|TRX||691365|P1|nnnnnnnnnn|||||ZZZ1";

export let scopes ="audience "

export function encode(hl7Message) {
    return b64encode(hl7Message, 'rawstd');
}

export function MedicationRequest(template)
{
    var res = template.replace("{{ timestamp }}", timestamp());
    return res;
}

function timestamp()
{
    var now = new Date(Date.now());
    var mon = now.getMonth() < 10 ? "0" + now.getMonth() : now.getMonth();
    var day = now.getDay() < 10 ? "0" + now.getDay() : now.getDay();
    var str = now.getFullYear() + "/" +   // oddly the time stamp in HL7v2 does not use en-CA as in "2012-12-20"
        mon + "/" + day + " " +
        now.toLocaleTimeString("en-CA");
    return str;
}
