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
export let Claim = [
    {
        name: "TACTDU_0104_REQUEST-319233",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~&\\|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|319233|P|2.1||\r" +
            "ZCA|1|70|04|AR|05|\r" +
            "ZCB|BC00000I10|140905|319233\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|319233||319233|2063735||100|7|91|nnnnnnnnnn|||||1486||222|||||nnnnnn|912351176\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 319233\r" +
            "ZZZ|TAC||319233|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||319233|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_0104_REQUEST-20334 (encrypted)",
        purpose: "Adjudicate a Dispense Claim request message (Encrypted)",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456|MOH|PP|${{ timestamp }}||ZPN|20334|P|2.1|\r" +
            "ZZZ|TAC||020618|P1|nnnnnnnnnn|||\r" +
            "ZZZ|TDU||020618|P1|nnnnnnnnnn||||\r" +
            "ZCA|000001|03|01|AR|02\r" +
            "ZCB|BC00000Z23|100409|020618\r" +
            "ZCC|0025Ü^`¸ûÍ¶š–Ë+Ž¾Ó/¸;Â%üõ]µp`Ñy7\r" +
            "ZCD|||R|000000146|03|000000146|00000027||000550|003|91|nnnnnnnnnn||||UF|005500|00000|00610|00000|00|00000|000000|03313\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^THESE ARE GENERIC DIRECTIONS\r\r"
    },
    {
        name: "TACTDU_01_REQUEST-319233-1",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|319233|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00000I10|140905|319233\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|319233||319233|2063735||100|7|91|nnnnnnnnnn|||||1486||222|||||nnnnnn|912351175\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 319233\r" +
            "ZZZ|TAC||319233|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||319233|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_01_REQUEST-319233-1",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|319233|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00000I10|140905|319233\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|319233||319233|2063735||100|7|91|nnnnnnnnnn|||||1486||222|||||nnnnnn|912351175\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 319233\r" +
            "ZZZ|TAC||319233|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||319233|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_01_REQUEST-324232",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|PHARMACY|PNP|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|324232|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00007310|141127|324232\r" +
            "ZCC||36876372|0009300001268||19530602|||XQNIJY|PJYJNNBD|000nnnnnnnnnn|F\r" +
            "ZCD|||N|454564||454564|2063735||30|10|91|nnnnnnnnnn||||NI|1486|12345|222|001|02||001|nnnnnn|912351285\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^12^123~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 324232\r" +
            "ZZZ|TAC||324232|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||324232|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_0104_REQUEST-356673",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|356673|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00000I10|140910|356673\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|356673||356673|2225972||1000|7|91|nnnnnnnnnn|||||1486||222|||||nnnnnn|912351275\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 356673\r" +
            "ZZZ|TAC||356673|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||356673|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_01_REQUEST-534532",
        purpose: "Adjudicate a Dispense Claim request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|PHARMACY|PNP|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|324232|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00007310|141122|534532\r" +
            "ZCC||||||||XQNIJY|PJYJNNBD|000nnnnnnnnnn|F\r" +
            "ZCD|||N|534532||534532|2063735||80|10|91|nnnnnnnnnn||||NI|1486|12345|222|||||nnnnnn|912351284\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^12^123~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 534532\r" +
            "ZZZ|TAC||534532|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||534532|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDU_11_REQUEST-319233-1",
        purpose: "Adjudicate a Dispense Claim Reversal request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|319233|P|2.1||\r" +
            "ZZZ|TAC||319233|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||319233|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|11|AR|05|\r" +
            "ZCB|BC00000I10|140905|319233\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|319233||319233|2063735||100|7|91|nnnnnnnnnn||||VX|1486||222|||||nnnnnn|912351177\r" +
            "ZCE|141020||||||||||||||||||\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^\r\r"
    },
    {
        name: "TACTDU_11_REQUEST-319233-2",
        purpose: "Adjudicate a Dispense Claim Reversal request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|123456|123456||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|319133|P|2.1||\r" +
            "ZZZ|TAC||319133|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||319133|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|11|AR|05|\r" +
            "ZCB|BC00000I10|140905|319133\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|319133||319133|2063735||100|7|91|nnnnnnnnnn||||VX|1486||222|||||nnnnnn|912351177\r" +
            "ZCE|141020||||||||||||||||||\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^\r\r"
    },
    {
        name: "TACTDU_11_REQUEST-345632",
        purpose: "Adjudicate a Dispense Claim Reversal request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|PHARMACY|PNP|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|324232|P|2.1||\r" +
            "ZZZ|TAC||345632|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|11|AR|05|\r" +
            "ZCB|BC00007310|141203|345632\r" +
            "ZCC|01||0009300000771|342|19211008|43534|1|FFCXG|BVHWO|000nnnnnnnnnn|F\r" +
            "ZCD|||N|345632||345632|2063735||||91|nnnnnnnnnn||||NI||||||||nnnnnn|912351290\r" +
            "ZCE|141204||||||||||||||||||\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 345632\r" +
            "ZZZ|TDU||345632|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDUTRP_01_REQUEST-324232",
        purpose: "Adjudicate a Dispense Claim TAC/TDU/TRP request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|PHARMACY|PNP|ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|324232|P|2.1||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00007310|141129|324232\r" +
            "ZCC||36876372|0009300001268||19530602|||XQNIJY|PJYJNNBD|000nnnnnnnnnn|F\r" +
            "ZCD|||N|454564||454564|2063735||30|10|91|nnnnnnnnnn||||NI|1486|12345|222|001|02||001|nnnnnn|912351289\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^12^123~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 324232\r" +
            "ZZZ|TAC||324232|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TDU||324232|P1|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZZZ|TRP||324232|P1|nnnnnnnnnn|||||ZZZ1^\r\r"
    },
    {
        name: "TACTDUTRP_01_REQUEST-433760",
        purpose: "Adjudicate a Dispense Claim TAC/TDU/TRP request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|433760|P|2.1||\r" +
            "ZZZ|TDU||433760|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|70|01|AR|05|\r" +
            "ZCB|BC00000I10|141014|433760\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|433760||433760|2332582||1000|35|91|nnnnnnnnnn|||||0100||860|||||nnnnnn|912351179\r" +
            "ZPM|PTSM|EDUC\r" +
            "ZZZ|TRP||433760|P1|nnnnnnnnnn||||\r" +
            "ZZZ|TAC||433760|P1|nnnnnnnnnn||||\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^Transaction Trace ID: 433760\r\r"
    }
];