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
export let MedicationStatement = [
    {
        name: "TDU_00_REQUEST - 674212",
        purpose: "DUE Inquiry request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|DESKTOP|EMR|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|674212|P|2.1||\r" +
            "ZCA|1|70|00|MA|01|\r" +
            "ZCB|LDJQQ|140715|674212\r" +
            "ZCC||||||||LYGYD|PQLZVK|000nnnnnnnnnn|M\r" +
            "ZCD||||319233|||66999997||100|25|||||||||||||||\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 674212\r" +
            "ZZZ|TDU||674212|P1|nnnnnnnnnn|||||ZZZ1^\r"
    },
    {
        name: "TDUTRP_00_REQUEST-332212",
        purpose: "DUE Inquiry TDU/TRP request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|332212|P|2.1||\r" +
            "ZZZ|TDU||332212|P1|nnnnnnnnnn||||\r" +
            "ZZZ|TRP||332212|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|70|00|AR|05|\r" +
            "ZCB|BC00007310|140912|332212\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD||||319233|||2326450||||||||||||||||||87168\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^\r"
    },
    {
        name: "TDUTRR_00_REQUEST-332214",
        purpose: "DUE Inquiry TDU/TRR request message",
        version: "PNet-R3",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP||userID:192.168.0.1|ZPN|332214|P|2.1||\r" +
            "ZZZ|TRR||332214|P1|nnnnnnnnnn||||\r" +
            "ZZZ|TDU||332214|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|70|00|AR|05|\r" +
            "ZCB|BC00007310|140912|332214\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD||||319233|||66123264||||||||||||||||||2915\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^\r"
    },
    {
        name: "TDUTRS_00_REQUEST-332251",
        purpose: "DUE Inquiry TDU/TRS request message",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP||userID:192.168.0.1|ZPN|332251|P|2.1||\r" +
            "ZZZ|TRS||332251|P1|nnnnnnnnnn||||\r" +
            "ZZZ|TDU||332251|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|70|00|AR|05|\r" +
            "ZCB|BC00007310|141012|332251\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD||||319233|||2063735||||||||||||||||||912351177\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^\r"
    },
    {
        name: "TRP_00_REQUEST - R3",
        purpose: "",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}|userID:192.168.0.1|ZPN|3365|P|2.1||\r" +
            "ZZZ|TRP||3365|P1|3E9V1|||PHSVE105|\r" +
            "ZCA||03|00|KC|13|ZCB|BC00007007|200916|3365|\r" +
            "ZCC||||||||||0009388880284|\r\r"
    },
    {
        name: "TRP_00_REQUEST - R3",
        purpose: "Patient Profile Request request message (NEXT initial request)",
        version: "PNet-R3",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|XXXXXXXXXXXXXXX|NA|PNP|PP||userID:192.168.0.1|ZPN|101159|P|2.1||\r" +
            "ZZZ|TRP||101159|P1|nnnnnnnnnn||||\r" +
            "ZCA|000001|03|00|AR|03|\r" +
            "ZCB|BC00000HC4|110401|101159\r" +
            "ZCC||||||||||000nnnnnnnnnn|F\r\r"
    },
    {
        name: "TRP_00_REQUEST - R3",
        purpose: "Patient Profile Request request message (NEXT part request)",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|XXXXXXXXXXXXXXX|NA|PNP|PP||userID:192.168.0.1|ZPN|101159|P|2.1||NEXT^ZCB^BC00000HC4^110401^101159\r\r"
    },
    {
        name: "TRP_00_REQUEST",
        purpose: "",
        version: "PNet-R3",
        message:
            "MSH|^~\\&|TRXTOOL|PCARESUP|PNP|PP|${{ timestamp }}|userID:192.168.0.1|ZPN|3371|P|2.1||\r" +
            "ZZZ|TRS||3371|P1|1D5T2|||RAHIMAN|\r" +
            "ZCA||03|00|KC|13|ZCB|BC00007007|200916|3371|\r" +
            "ZCC||||||||||0009427405543|\r\r"
    },
    {
        name: "TRP_00_REQUEST - 248875",
        purpose: "Patient Profile Request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567||EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|248875|P|2.1||\r" +
            "ZZZ|TRP||248876|91|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|00|QA|01|\r" +
            "ZCB|QAEMRMD|140530|248876\r" +
            "ZCC||||||||||000xxxxxxxxxx |\r\r"
    },
    {
        name: "TRP_00_REQUEST - 444333",
        purpose: "Patient Profile Request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|444333|P|2.1||\r" +
            "ZZZ|TRP||444333|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA||70|00|QA|02|\r" +
            "ZCB|QAERXPP|111206|235354\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r\r"
    },
    {
        name: "TRP_00_REQUEST-613252",
        purpose: "Patient Profile Request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|EMR|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|613252|P|2.1||\r" +
            "ZZZ|TRP||613252|P1|nnnnnnnnnn||||\r" +
            "ZCA||70|00|MA|01|\r" +
            "ZCB|LDJQQ|140806|613252\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r\r"
    },
    {
        name: "TRR_00_REQUEST-235348",
        purpose: "Patient Profile Most Recent Only Request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|235348|P|2.1||\r" +
            "ZZZ|TRR||235348|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA||70|00|KC|13|\r" +
            "ZCB|BYR|111006|235348\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r\r"
    }
];