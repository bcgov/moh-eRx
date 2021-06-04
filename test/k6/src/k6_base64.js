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

import * as common from './inc/common.js';

export default function() {

    var payload = 'TVNIfF5+XCZ8VFJYVE9PTHxQQ0FSRVNVUHxQTlB8UFB8MjAyMS8wNi8wNCAwMToxMDoxMHx8WlBOfDkyODZ8UHwyLjF8fA1aWlp8VERSfHw5Mjg2fFAxfDJGM1AyfHx8fA1aQ0F8fDAzfDAwfEtDfDEzfA1aQ0J8QkMwMDAwNzAwN3wyMDEyMjJ8OTI4Ng1aUEN8MjI0MDU3OXx8fHx8fFl8WlBDMV5eXjc2NjcyMA0N';

    var url = common.MedicationServiceUrl;
    var scopes = "system/Medication.read";
    common.authorizeClient(scopes);

    common.submitHL7MessageBase64(url, payload);
}