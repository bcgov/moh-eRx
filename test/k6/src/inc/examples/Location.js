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
export let Location = [
    {
        name: "TIL_00_REQUEST-1111",
        purpose: "Get Location Details request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||EMRMD|${{ timestamp }}||ZPN|1111|P|2.1||\r" +
            "ZCA||70|00|MA|01|\r" + 
            "ZCB|MDA|120113|1111\r" + 
            "ZPL|QAERXPP||||||||||||||MM\r" + 
            "ZZZ|TIL||1111|P1|nnnnnnnnnn|||||ZZZ1\r\r"
    }
];