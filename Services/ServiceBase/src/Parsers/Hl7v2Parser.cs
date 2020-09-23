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
    using System.Text;
    using Health.PharmaNet.Common.Authorization;

    using HL7.Dotnetcore;

    /// <summary>
    /// The Pharmanet Fhir Parser helper.
    /// </summary>
    public static class HL7v2Parser
    {
        /// <summary>
        /// Parse a base64 encoded string into a HL7-v2 Message object.
        /// </summary>
        /// <param name="base64EncodedData">A base64 stream to be parsed into a Hl7-v2 object.</param>
        /// <returns>A HL7.Dotnetcore.Message instance.</returns>
        public static Message ParseBase64EncodedData(string base64EncodedData)
        {
            if (!string.IsNullOrEmpty(base64EncodedData))
            {
                string messageString = Encoding.UTF8.GetString(System.Convert.FromBase64String(base64EncodedData));

                Message message = new Message(messageString);
                if (!message.ParseMessage())
                {
                    throw new ArgumentException("Failed to parse ${1}", nameof(base64EncodedData));
                }

                return message;
            }

            throw new ArgumentNullException(nameof(base64EncodedData));
        }

        /// <summary>
        /// Parse a base64 encoded string into a HL7-v2 Message object.
        /// </summary>
        /// <param name="messageString">A string containing HL7v2 message to be parsed into a Hl7-v2 object.</param>
        /// <returns>A HL7.Dotnetcore.Message instance.</returns>
        public static Message ParseString(string messageString)
        {
            if (!string.IsNullOrEmpty(messageString))
            {
                Message message = new Message(messageString);

                if (!message.ParseMessage())
                {
                    throw new ArgumentException("Failed to parse ${1}", nameof(messageString));
                }

                return message;
            }

            throw new ArgumentNullException(nameof(messageString));
        }

        /// <summary>
        /// Get the MessageType from the HL7-v2 Message Header field MSH.9.
        /// </summary>
        /// <param name="message">An hl7-v2 Message.</param>
        /// <returns>A string containing the HL7-v2 MSH.9 MessageType value.</returns>
        public static MessageType GetMessageType(Message? message)
        {
            string messageType = message!.GetValue("MSH.9");
            string controlId = message!.GetValue("MSH.10");

            if (message.IsComponentized("MSH.9"))
            {
                messageType = message.GetValue("MSH.9.1");
            }

            return new MessageType(messageType, controlId);
        }
    }
}
