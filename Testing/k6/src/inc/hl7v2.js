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

let SEPARATOR = "\r";

/*export let MedicationRequest_ZPN_TRX_X0 = "MSH|^~\&|DESKTOP|EMR|DESKTOP|EMRMD|||ZPN^^|691365|P|2.1||" + "\r" +
"ZCA||70|X0|MA|01|" + "\r" +
"ZCB|LDJQQ|140827|691365"  + "\r" +
"ZCC||||||||||000nnnnnnnnnn|"  + "\r" +
"ZPR|||||||||||"  + "\r" +
"ZZZ|TRX||691365|P1|nnnnnnnnnn|||||ZZZ1"; */

export let MedicationRequest_ZPN_TRX_X0 = 
"MSH|^~\&|1234567|1234567||EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|1112|P|2.1||" + SEPARATOR +
"ZCA||70|X0|QA|01|" + SEPARATOR +
"ZCB|QAEMRMD|210317|1112" + SEPARATOR +
"ZCC||||||||||0009698713408|" + SEPARATOR+
"ZPR|||||||||||" + SEPARATOR +
"ZZZ|TRX||1112|91|XXANV|||||ZZZ1^" + SEPARATOR;

export let MedicationRequest_ZPN_TRX_X0_sample2 = 
"MSH|^~\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|125|P|2.1||"  + SEPARATOR +
"ZCA||70|X0|M1|04|"  + SEPARATOR +
"ZCB|AAA|110916|0215"  + SEPARATOR +
"ZCC||||||||||000nnnnnnnnnn|"  + SEPARATOR +
"ZPR|1047||||||||||"  + SEPARATOR +
"ZZZ|TRX||545132|91|nnnnnnnnnn||||" + SEPARATOR;

export let MedicationStatement_ZPN_TRP_00 = 
"MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}|userID:192.168.0.1|ZPN^^|3365|P|2.1||" + SEPARATOR + 
"ZZZ|TRP||3365|P1|3E9V1|||PHSVE105|" + SEPARATOR +
"ZCA||03|00|KC|13|ZCB|BC00007007|200916|3365|" + SEPARATOR + 
"ZCC||||||||||0009388880284|";

export let MedicationStatement_ZPN_TRS_00 = 
"MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|||ZPN^^|3371|P|2.1||" + SEPARATOR +
"ZZZ|TRS||3371|P1|1D5T2|||RAHIMAN|" + SEPARATOR +
"ZCA||03|00|KC|13|ZCB|BC00007007|200916|3371|" + SEPARATOR +
"ZCC||||||||||0009427405543|";


export let Medication_ZPN_TDR = 
"MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|||ZPN|9286|P|2.1||" + SEPARATOR + 
"ZZZ|TDR||9286|P1|2F3P2||||" + SEPARATOR +
"ZCA||03|00|KC|13|" + SEPARATOR +
"ZCB|BC00007007|201222|9286" + SEPARATOR +
"ZPC|2240579||||||Y|ZPC1^^^76672";

export let Patient_ZPN_TID_00 = 
"MSH|^~\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}|userID:192.168.0.1|ZPN^^|3362|P|2.1||" + SEPARATOR +
"ZZZ|TID||3362|P1|6H2O2||" + SEPARATOR +
"ZCA||03|00|KC|13ZCB|BC00007007|200916|3362|" + SEPARATOR + 
"ZCC||||||||||0009433498542|";


function encode(hl7Message) {
    return b64encode(hl7Message, 'std');
}

export function Hl7v2RequestEncoded(template)
{
    var res = template.replace("${{ timestamp }}", timestamp());
    return encode(res);
}

function timestamp()
{
    var now = new Date(Date.now());
    var mon = now.getMonth() + 1;
    mon = (mon < 10) ? "0" + mon : mon;
    var day = now.getDate();
    day = (day < 10) ? "0" + day : day
    var str = now.getFullYear() + "/" +   // oddly the time stamp in HL7v2 does not use en-CA as in "2012-12-20"
        mon + "/" + day + " " +
        now.toLocaleTimeString("en-CA");
    return str;
}
