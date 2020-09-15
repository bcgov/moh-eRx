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
namespace Health.PharmaNet.Models
{
    using System.Text;
    using System.Text.RegularExpressions;

    using Hl7.Fhir.Model;

    /// <summary>
    /// The Pharmanet Adapter to convert from/to message models.
    /// </summary>
    public static class PharmanetAdapter
    {
        private static readonly string HL7v2ContentType = "x-application/hl7-v2+er7";

        /// <summary>
        /// Converts a PharmanetMessage to an HL7 FHIR DocumentReference model.
        /// </summary>
        /// <param name="messageModel">The PharmanetMessage to convert from.</param>
        /// <param name="requestDocumentReference">The optional DocumentReference this message is in response to.</param>
        /// <returns>Returns a new DocumentReference from the PharmanetMessage. 
        /// If the requestDocumentReference is provided, it used that to fill out cross-referenced data/
        /// </returns>

        public static DocumentReference FromPharmanetMessage(PharmanetMessage messageModel, DocumentReference? requestDocumentReference = null)
        {
            DocumentReference documentReference = new DocumentReference();

            DocumentReference.ContentComponent item = new DocumentReference.ContentComponent();
            item.Attachment.Data = Encoding.UTF8.GetBytes(messageModel.Hl7Message);
            item.Attachment.ContentType = HL7v2ContentType;

            documentReference.Content.Add(item);

            return documentReference;
        }

        /// <summary>
        /// Converts an HL7 FHIR DocumentReference model to a PharmanetMessage model.
        /// </summary>
        /// <param name="documentReference">The DocumentReference to convert from.</param>
        /// <returns>Returns a new PharmanetMessage mapped from the provided DocumentReference.</returns>

        public static PharmanetMessage FromDocumentReference(DocumentReference documentReference)
        {
            PharmanetMessage messageModel = new PharmanetMessage();

            // HL7 FHIR spec for GUID/UUID has this mandatory prefix in the value field.
            foreach( Match? m in Regex.Matches(documentReference.MasterIdentifier.Value, @"urn:uuid:(\s+)"))
            {
                GroupCollection groups = m!.Groups;
                string value = groups[0].Value;
                messageModel.TransactionId = value; // The GUID/UUID
                break;
            }

            DocumentReference.ContentComponent[] content = documentReference.Content.ToArray();

            byte[] data = content[0].Attachment.Data;
            string contentType = content[0].Attachment.ContentType;

            bool good = ((data.Length>0) && contentType.Equals(HL7v2ContentType));
            messageModel.Hl7Message = (good) ? Encoding.UTF8.GetString(data) : string.Empty;

            return messageModel;
        }
    }
}
