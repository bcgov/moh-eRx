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
    using Health.PharmaNet.Common.Authorization;

    using HL7Enumerator; // Galkam.HL7Enumerator;

    /// <summary>
    /// The Pharmanet Fhir Parser handler.
    /// </summary>
    public static class HL7v2ParsingHandler
    {
        /// <summary>
        /// Get the MessageType from the HL7-v2 Message Header field MSH.9.
        /// </summary>
        /// <param name="message">An hl7-v2 Message.</param>
        /// <returns>A MessageType object containing the HL7-v2 MSH.9 MessageType and ControlId.</returns>
        public static MessageType GetMessageType(string? message)
        {
            string sender = HL7Message.ParseOnly(message, "MSH.3");
            string facility = HL7Message.ParseOnly(message, "MSH.4");
            string receivingapp = HL7Message.ParseOnly(message, "MSH.5");
            string datetime = HL7Message.ParseOnly(message, "MSH.7");
            string security = HL7Message.ParseOnly(message, "MSH.8");
            string messageType = HL7Message.ParseOnly(message, "MSH.9");
            string controlId = HL7Message.ParseOnly(message, "MSH.10");

            return new MessageType(messageType, controlId);
        }
    }
}
