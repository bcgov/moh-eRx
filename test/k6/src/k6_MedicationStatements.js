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
import * as examples from './inc/examples/MedicationStatement.js';

export default function() {

    var url = common.MedicationStatementServiceUrl;
    var scopes = "system/MedicationStatement.read";
    common.authorizeClient(scopes);

    examples.MedicationStatement.forEach(msg => {
        common.submitMessage(url, msg);
        sleep(1);
    });
}