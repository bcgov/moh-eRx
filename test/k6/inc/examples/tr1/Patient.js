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

export const examples = [
    {
        name: "TID_00_REQUEST - 443769",
        purpose: "Patient Identification request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|443769|P|2.1||\r" +
            "ZCA||70|00|AR|05|\r" +
            "ZCB|BC00000I10|140715|443769\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZZZ|TID||443769|P1|nnnnnnnnnn|||\r\r"
    },
    {
        name: "TPA_00_REQUEST - 819848",
        purpose: "Patient Address Update request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|819848|P|2.1||\r" +
            "ZZZ|TPA||819848|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA||70|00|AR|05|\r" +
            "ZCB|BC00000I10|140912|819848\r" +
            "ZCC|||||19620524|||JZHWB|KXOFKLEU|000nnnnnnnnnn|F\r" +
            "ZPA|AL|BRADFORD|L|ZPA1^^^^^|ZPA2^M^^^^^109 HAMSTEAD CLOSE NW^^VANCOUVER^^V9D4E5^^BC^^^^^^^^^^\r"
    },
    {
        name: " TPH_00_REQUEST - 62028",
        purpose: "Patient PHN Assignment request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|62028|P|2.1||\r" +
            "ZZZ|TPH||62028|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA||70|00|AR|05|\r" +
            "ZCB|BC00000I10|140909|62028\r" +
            "ZCC|||||19681025||||||M\r" +
            "ZPA|ARNOLD|QATAR||ZPA1^^^^^|ZPA2^M^^^^^6615 VEDDER RD^^SARDIS^^V2R1C9^^BC^^^^^^^^^^\r"
    },
    /*{
        // As of May 18, 2021, PharmaNet dev env does not reply to this.
        name: "TID_00_REQUEST - R3",
        purpose: "Patient Identification request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}|userID:192.168.0.1|ZPN|3362|P|2.1||\r" +
            "ZZZ|TID||3362|P1|6H2O2||\r" +
            "ZCA||03|00|KC|13ZCB|BC00007007|200916|3362|\r" +
            "ZCC||||||||||0009433498542|\r\r"
    },*/
    {
        name: "TPI_00_REQUEST - 150146",
        purpose: "Patient Medication Profile Update request message",
        version: "PNet-R70",
        message:
            "MSH|^~\&|DESKTOP|EMR|DESKTOP|EMRMD|2014/09/09 12:22:22|userID:192.168.0.1|ZPN|150146|P|2.1||\r" +
            "ZZZ|TPI||150146|P1|nnnnnnnnnn||||\r" +
            "ZCA||70|00|MA|01|\r" +
            "ZCB|LDJQQ|140910|150146\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPB|ZPB1^^^^^^^^|ZPB2^120898^^^^PH^20140902^^^^\r"
    }
];
