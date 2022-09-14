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
export let Consent = [
    {
        name: "TCP_00_REQUEST-998015",
        purpose: "Patient Keyword Maintenance request message",
        version: "PNet-R70",
        message:
            "MSH|^~\\&|1234567|1234567||ERXPP|${{ timestamp }}|userID:192.168.0.1|ZPN|998015|P|2.1||\r" +
            "ZZZ|TCP||998015|P1|nnnnnnnnnn||||KEYWORD|ZZZ1^\r" +
            "ZCA||70|00|AR|05|\r" +
            "ZCB|BC00000I10|140802|998015\r" +
            "ZCC||||||||||000nnnnnnnnnn|\r\r"
    }
];