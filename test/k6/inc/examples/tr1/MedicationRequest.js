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
        name: "TRX_X0_REQUEST -  125",
        purpose: "Retrieve Patient Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|1112|P|2.1||\r" +
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
            "MSH|^~\\&|DESKTOP|EMR|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|691365|P|2.1||\r" +
            "ZCA||70|X0|MA|01|\r" +
            "ZCB|LDJQQ|140827|691365\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPR|||||||||||\r" +
            "ZZZ|TRX||691365|P1|nnnnnnnnnn|||||ZZZ1\r\r"
    },
    {
        name: "TRX_X0_REQUEST - 125",
        purpose: "Retrieve Patient Prescription request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|125|P|2.1||\r" +
            "ZCA||70|X0|M1|04|\r" +
            "ZCB|AAA|110916|0215\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPR|1047||||||||||\r" +
            "ZZZ|TRX||545132|91|nnnnnnnnnn||||\r\r"
    },
    {
        name: "TRX_X1_REQUEST - 100001",
        purpose: "Record a Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|BC01111111|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|100001|P|2.1|123456|\r" +
            "ZCA|123456|70|X1|MA|01|123456\r" +
            "ZCB|LDJQQ|140817|100001\r" +
            "ZCC|12|123456|000nnnnnnnnnn|123|19671218|12345|1|JFIRKPYGI|ZRNCZJ|000nnnnnnnnnn|F\r" +
            "ZPX|ZPX1^Y^91^nnnnnaaaaa^A^ER^LDJQQ^LDJQQ^LDJQQ^100002^^^20140817^30010101^20140817^2042622^507^ISORDIL 10 TITRADOSE TABLETS 10MG                               ^4000^400^3^0^1000^10^003.8^1^QD^10^DOS^A^TESTING^TESTING^N^N^Y^N^N^N^N^N^N^USE ONCE DAILY^TESTING^123456^NR^PTSM^XCHGD^LDJQQ^2042622^^TESTING^TESTING^TESTING^^TESTING^100001^2014/09/02 16:39:52\r" +
            "ZZZ|TRX|A|100001|91|nnnnnnnnnn|1|TESTING|||ZZZ1^TESTING\r\r"
    },
    {
        name: "TRX_X1_REQUEST - 195232",
        purpose: "Record a Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|195232|P|2.1||\r" +
            "ZCA|1|70|X1|AR|05|\r" +
            "ZCB|BC00000I10|140825|195232\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPX|ZPX1^Y^91^22441XTARS^^^^^^19523342^^^20140825^^20140825^2063735^^ALDOMET^100^100^0^100^100^^^1^QD^^^^^^^^^^^^^^^PATIENT INSTRUCTIONS^^^^^^^^^^PATIENT INSTRUCTIONS^^^^195232^\r" +
            "ZZZ|TRX||195232|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TRX_X1_REQUEST - 195233",
        purpose: "Record a Prescription request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|1234567|1234567|1234567|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|195233|P|2.1||\r" +
            "ZCA|1|70|X1|AR|05|123456\r" +
            "ZCB|BC00007310|150301|195232\r" +
            "ZCC|12|1234567890|000nnnnnnnnnn|123|19690619|12345|1|QIAFCQGU|XY|000nnnnnnnnnn|M\r" +
            "ZPX|ZPX1^Y^91^1J7S2QMJJ^^^^^^43542368^^^20150228^^20150228^2063735^4679^PMS-BACLOFEN TAB 10MG     ^ 1500 ^ 300 ^ 1 ^ 0 ^ 750 ^ 15 ^ 001.9^ 3 ^ QD ^ 100 ^ G ^ 1 ^ THIS IS A TEST^ THIS IS A TEST ^ N ^ N ^ Y ^ N ^ Y ^ N ^^^^ PATIENT INSTRUCTIONS ^ PRESCRIBER NOTES ^ 1234567890 ^ NI ^ ADD ^ CHGD ^^^^^ PATIENT INSTRUCTIONS ^^^^ 195232 ^\r" +
            "ZZZ|TRX||195232|P1|nnnnnnnnnn||THIS IS A TEST|||ZZZ1^\r\r"
    },
    {
        name: "TRX_X1_REQUEST - 543221",
        purpose: "Record a Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|543221|P|2.1||\r" +
            "ZCA||70|X1|ZD|06|\r" +
            "ZCB|BC00000F34|110920|543221\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPX|ZPX1^Y^P1^00083^^^^^^543421^^^20110920^^20110920^2013983^^BACITRACIN/POLYMYXIN B SULFATE^3000^10^1^^20^^^2^QD^^^^^^^^^^^^^^^TAKE WITH WATER^^^^^^^^^^^^^^^\r" +
            "ZZZ|TRX||000001|91|nnnnnnnnnn|||||ZZZ1^\r"
    },
    {
        name: "TRX_X2_REQUEST - 00001",
        purpose: "Update a Prescription Status request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|00001|P|2.1||\r" +
            "ZCA||70|X2|M1|01|\r" +
            "ZCB|AAA|111012|00001\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZPX|ZPX1^^^^R^ER^^^^^9185^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\r" +
            "ZZZ|TRX||00001|91|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TRX_X3_REQUEST - 100004",
        purpose: "Adapt a Prescription request message",
        version: "PNet-R70",
        includeIdentifier: false,
        message:
            "MSH|^~\\&|DESKTOP|BC01111111|DESKTOP|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|100004|P|2.1|123456|\r" +
            "ZCA|123456|70|X3|AR|05|123456\r" +
            "ZCB|BC00000I10|140818|100004\r" +
            "ZCC|12|123456|000nnnnnnnnnn|123|19671218|12345|1|JFIRKPYGI|ZRNCZJ|000nnnnnnnnnn|F\r" +
            "ZPX|ZPX1^Y^P1^nnnnnaaaaa^A^ER^BC00000I10^BC00000I10^BC00000I10^100004^912351182^912351182^20140817^30010101^20140817^2042622^507^ISORDIL 10 TITRADOSE TABLETS 10MG                               ^ 4000 ^ 400 ^ 3 ^ 0 ^ 1000 ^ 2 ^ 001.9    ^ 99 ^ QD ^ 10 ^ EA ^ 2 ^ TESTING ^ TESTING ^ N ^ N ^ Y ^ N ^ N ^ N ^ N ^ Y ^ N ^ TAKE ONCE DAILY^ TESTING ^ 123456 ^ VY ^ PTSM ^ CHGD ^ BC00000I10 ^ 2042622 ^ 20140808 ^ TESTING ^ TESTING ^ TESTING ^ Y ^ RGCDVXKLP ^ 613045 ^\r" +
            "ZZZ|TRX|A|100004|P1|nnnnnnnnnn|1|TESTING|||ZZZ1^\r\r"
    },
    {
        name: "TRX_X4_REQUEST - 000001",
        purpose: "Retrieve Prescriber Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|000001|P|2.1||\r" +
            "ZCA|01|70|X4|M1|04|\r" +
            "ZCB|AAA|111028|000001\r" +
            "ZPR|9474|P1|00189||||||||\r" +
            "ZZZ|TRX||000001|91|nnnnnnnnnn|||||ZZZ1^\r"
    },
    {
        name: "TRX_X4_REQUEST - 177917",
        purpose: "Retrieve Prescriber Prescription request message",
        version: "PNet-R70",
        message:
            "MSH|^~\&|DESKTOP|EMR|DESKTOP|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|177917|P|2.1||\r" +
            "ZCA|1|70|X4|MA|01|\r" +
            "ZCB|LDJQQ|140624|177917\r" +
            "ZPR||91|29730||||||||\r" +
            "ZZZ|TRX||177917|91|nnnnnnnnnn|||||ZZZ1^\r"
    }
];
