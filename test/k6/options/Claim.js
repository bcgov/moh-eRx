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

// these are the options that are read by the k6 test framework
// here you can define the vus, iterations, duration, or stages
// see https://k6.io/docs/using-k6/k6-options/ for more information
export const options = {
    stages: [
        { duration: "20m", target: 30 },
        { duration: "20m", target: 30 },
        { duration: "5m", target: 0 },
    ],
};

// the mean time delay in seconds between each transaction
export const meanDelaySeconds = 0.21;
