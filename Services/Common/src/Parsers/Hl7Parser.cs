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
namespace Health.PharmaNet.Parsers
{
    using System;

    using HL7.Dotnetcore;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Pharmanet Delegate, which communicates directly to the Pharmanet proxy service.
    /// </summary>
    public class Hl7Parser : IHl7Parser
    {
        private readonly ILogger<Hl7Parser> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hl7Parser"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        public Hl7Parser(ILogger<Hl7Parser> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Message ParseV2String(string messageString)
        {
            if (!string.IsNullOrEmpty(messageString))
            {
                Message message = new Message(messageString);

                try
                {
                    if (!message.ParseMessage())
                    {
                        throw new ArgumentException("HL7v2 Parsing Error.");
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = "HL7v2 Exception: " + ex.Message + "for HL7:" + messageString;
                    throw new ArgumentException(errMsg, ex);
                }

                return message;
            }

            throw new ArgumentNullException(nameof(messageString));
        }

        /// <inheritdoc/>
        public DocumentReference ParseFhirJson(string json)
        {
            FhirJsonParser parser = new FhirJsonParser(new ParserSettings { AcceptUnknownMembers = true, AllowUnrecognizedEnums = true });
            DocumentReference documentReference = parser.Parse<DocumentReference>(json);
            return documentReference;
        }
    }
}
