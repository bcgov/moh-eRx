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
export let MedicationRequest = [
    {
        name: "TRX_X0_REQUEST -  125",
        purpose: "Retrieve Patient Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|1112|P|2.1||\r" +
            "ZCA||70|X0|QA|01|\r" +
            "ZCB|QAEMRMD|210317|1112\r" +
            "ZCC||||||||||0009698713408|\r" +
            "ZPR|||||||||||\r" +
            "ZZZ|TRX||1112|91|XXANV|||||ZZZ1^\r\r"
    },
    {
        name: "TRX_X0_REQUEST-691365",
        purpose: "Retrieve Patient Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|EMR|DESKTOP|EMRMD|2014/09/09 12:11:11|userID:192.168.0.1|ZPN|691365|P|2.1||\r" +
            "ZCA||70|X0|MA|01|\r" +
            "ZCB|LDJQQ|140827|691365\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPR|||||||||||\r" +
            "ZZZ|TRX||691365|P1|nnnnnnnnnn|||||ZZZ1\r\r"
    }
];