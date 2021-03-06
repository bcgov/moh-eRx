//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
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
namespace PharmaNet.Client.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    public class PharmanetService : IPharmanetService
    {
        private readonly IConfiguration configuration;
        private readonly IAuthService authService;


        public PharmanetService(IConfiguration configuration,
            IAuthService authService)
        {
            this.configuration = configuration;
            this.authService = authService;
        }

        public string PatientQuery()
        {
            return string.Empty;
        }


    }
}