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
export let Patient = [
    {
        name: "TID_00_REQUEST - R3",
        purpose: "Patient Identification request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}||ZPN|3362|P|2.1||\r" +
            "ZZZ|TID||3362|P1|6H2O2||\r" +
            "ZCA||03|00|KC|13ZCB|BC00007007|200916|3362|\r" +
            "ZCC||||||||||0009433498542|\r\r"
    },
    {
        name: "TID_00_REQUEST - 443769",
        purpose: "Patient Identification request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP||userID:192.168.0.1|ZPN|443769|P|2.1||\r" +
            "ZCA||70|00|AR|05|\r" +
            "ZCB|BC00000I10|140715|443769\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZZZ|TID||443769|P1|nnnnnnnnnn|||\r\r"
    }
];