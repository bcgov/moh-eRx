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

    var payload = 'TVNIfF5-XCZ8REVTS1RPUHxCQzAxNDAwNTAzfFBOUHxJUHx8RFJCOjIxNi4yMzIuMTMyLjEzN3xaUE58MTAzODQ5fFB8Mi4xDVpaWnxURFJ8fDEwMzg0OXxQMXwxMjM0NXx8fA1aQ0F8MDAwMDAxfDcwfDAwfEFSfDA3DVpDQnxCQzAwMDAwWlpafDIxMDYwM3wxMDM4NDkNWlBDfDMzNzczMHx8fHx8fE58WlBDMV5GREJeRURVQ0xPTkcNDQ';

    var url = common.MedicationServiceUrl;
    var scopes = "system/Medication.read";
    common.authorizeClient(scopes);

    common.submitHL7MessageBase64(url, payload);
}