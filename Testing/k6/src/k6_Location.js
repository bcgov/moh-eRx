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

import { sleep } from 'k6';
import * as common from './inc/common.js';
import * as hl7 from './inc/hl7v2.js';

export default function() {

    var url = common.LocationServiceUrl;
    var payload = hl7.Hl7v2RequestEncoded(hl7.Location_TIL_00_50); // Returns Base64 encoded hl7v2 message
    var scopes = "openid audience system/MedicationRequest.write system/MedicationRequest.read";
    common.authorizeClient(scopes);
    common.postMessage(url, payload);
}