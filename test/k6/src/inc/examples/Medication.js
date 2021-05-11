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
export let Medication = [
    {
        name: "TDR_50_REQUEST - R3",
        purpose: "Drug Monograph Information request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}||ZPN|9286|P|2.1||\r" +
            "ZZZ|TDR||9286|P1|2F3P2||||\r" +
            "ZCA||03|00|KC|13|\r" +
            "ZCB|BC00007007|201222|9286\r" +
            "ZPC|2240579||||||Y|ZPC1^^^766720\r\r"

    },
    {
        name: "TDR_50_REQUEST - 149966",
        purpose: "Drug Monograph Information request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|149966|P|2.1||\r" +
            "ZZZ|TDR||149966|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA||70|00|KC|13|\r" +
            "ZCB|BC00000F07|111205|149966\r" +
            "ZPC|122858||||||N|ZPC1^FDB^ADIMONOG^0\r"

    },
];