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
export let MedicationDispense = [
    {
        name: "TMU_01_REQUEST - R70",
        purpose: "Medication Update request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|EMR|DISMEDUPDATE|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|119908|P|2.1||\r" +
            "ZZZ|TMU||119908|91|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|01|MA|01|\r" +
            "ZCB|LDJQQ|140724|119908\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|126599||126599|2225972||1000|100|91|nnnnnnnnnn||||||||||||nnnnnn|\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^TMU 0\r\r"
    },
    {
        name: "TMU_01_REQUEST - 1111",
        purpose: "Medication Update request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567|1234567|PP|${{ timestamp }}|userID:192.168.0.1|ZPN|1111|P|2.1||\r" +
            "ZZZ|TMU||1111|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|03|01|NA|01|\r" +
            "ZCB|CAS|110614|1111\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|1111||1111|18007||100|100|91|24274FOLEY||||||||||||18007\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^1ST DR\r\r"
    },
    {
        name: "TMU_01_REQUEST - 119908",
        purpose: "Medication Update request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|DESKTOP|EMR|DISMEDUPDATE|EMRMD|${{ timestamp }}|userID:192.168.0.1|ZPN|119908|P|2.1||\r" +
            "ZZZ|TMU||119908|91|nnnnnnnnnn|||||ZZZ1^\r" +
            "ZCA|1|70|01|MA|01|\r" +
            "ZCB|LDJQQ|140724|119908\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD|||N|126599||126599|2225972||1000|100|91|nnnnnnnnnn||||||||||||nnnnnn|\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^TMU 0\r\r"
    },
    {
        name: "TMU_11_REQUEST - 58709",
        purpose: " Medication Update Reversal request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567|1234567|OP|${{ timestamp }}|userID:192.168.0.1|ZPN|58709|P|2.1||\r" +
            "ZZZ|TMU||58709|P1|nnnnnnnnnn||||\r" +
            "ZCA|1|03|11|NA|01|\r" +
            "ZCB|CAS|110414|58709\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r" +
            "ZCD||||58709||58709|120375||1000|100|91|nnnnnnnnnn||||NK||||||||nnnnnn\r" +
            "ZPJ|ZPJ1^^^^^^|ZPJ2^^~ZPJ2^^~ZPJ2^^|ZPJ3^^|ZPJ4^TMU01\r"
    }
];