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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Hl7.Fhir.Model;

    /// <summary>
    /// The Pharmanet Adapter to convert from/to message models.
    /// </summary>
    public static class PharmanetDelegateAdapter
    {
        /// <summary>
        /// The standard ContentType ("Mime Type") for HL7-v2 payload.
        /// </summary>
        public const string HL7v2ContentType = @"x-application/hl7-v2+er7";

        /// <summary>
        /// The standard urn prefix when using UUID/GUID for identifiers in HL7 FHIR.
        /// </summary>
        public const string MasterIdentifierUrnPrefix = @"urn:uuid:";

        /// <summary>
        /// The regular expression pattern to use to extract just the UUID from the master identifier.
        /// </summary>
        private const string MasterIdentifierPattern = @"^urn:uuid:(.*)$";

        /// <summary>
        /// The system value of urn:ietf:rfc:3986 for when the value of the identifier is itself a globally unique URI.
        /// </summary>
        private const string MasterIdentifierNamespace = @"urn:ietf:rfc:3986";

        /// <summary>
        /// Converts a PharmanetMessage to an HL7 FHIR DocumentReference model.
        /// </summary>
        /// <param name="messageModel">The PharmanetMessage to convert from.</param>
        /// <param name="related">The optional ResourceReference representing a reference to the request message.</param>
        /// <returns>Returns a new DocumentReference from the PharmanetMessage.
        /// If the requestDocumentReference is provided, it used that to fill out cross-referenced data.
        /// </returns>
        public static DocumentReference ToDocumentReference(PharmanetMessageModel messageModel, ResourceReference? related = null)
        {
            DocumentReference documentReference = new DocumentReference();

            documentReference.Status = DocumentReferenceStatus.Current;
            documentReference.Date = DateTime.UtcNow;
            documentReference.Id = messageModel.TransactionId; // set the GUID as the base artefact ID.
            documentReference.MasterIdentifier = new Identifier(MasterIdentifierNamespace, MasterIdentifierUrnPrefix + messageModel.TransactionId);
            DocumentReference.ContentComponent item = new DocumentReference.ContentComponent();
            item.Attachment = new Attachment();
            string hl7Message = Encoding.UTF8.GetString(Convert.FromBase64String(messageModel.Hl7Message));
            item.Attachment.Data = Encoding.UTF8.GetBytes(hl7Message);
            item.Attachment.ContentType = HL7v2ContentType;

            documentReference.Content.Add(item);
            if (related != null)
            {
                documentReference.Context = new DocumentReference.ContextComponent();
                documentReference.Context.Related = new List<ResourceReference>();
                documentReference.Context.Related.Add(related);
            }

            return documentReference;
        }

        /// <summary>
        /// Converts an HL7 FHIR DocumentReference model to a PharmanetMessage model.
        /// </summary>
        /// <param name="documentReference">The DocumentReference to convert from.</param>
        /// <param name="base64Encode">A value indicating whether to base64 encode the HL7v2.</param>
        /// <returns>Returns a new PharmanetMessage mapped from the provided DocumentReference.</returns>
        public static PharmanetMessageModel ToPharmanetMessageModel(DocumentReference documentReference, bool base64Encode)
        {
            PharmanetMessageModel messageModel = new PharmanetMessageModel();

            // HL7 FHIR spec for GUID/UUID has this mandatory prefix in the value field.
            if (documentReference.MasterIdentifier != null)
            {
                foreach (Match? m in Regex.Matches(documentReference.MasterIdentifier.Value, MasterIdentifierPattern))
                {
                    GroupCollection groups = m!.Groups;
                    string value = groups[1].Value;
                    messageModel.TransactionId = value; // The GUID/UUID
                    break;
                }
            }
            else
            {
                // If no masterIdentifier is given, generate a new one
                messageModel.TransactionId = System.Guid.NewGuid().ToString();
            }

            DocumentReference.ContentComponent[] content = documentReference.Content.ToArray();

            byte[] data = content[0].Attachment.Data;
            string contentType = content[0].Attachment.ContentType;
            bool good = (data.Length > 0) && contentType.Equals(HL7v2ContentType, System.StringComparison.Ordinal);

            messageModel.Hl7Message = good ? (base64Encode ? Convert.ToBase64String(data, Base64FormattingOptions.None) : Encoding.UTF8.GetString(data)) : string.Empty;

            return messageModel;
        }

        /// <summary>
        /// Builds a ResourceReference from a DocumentReference to add to related field in the response.
        /// </summary>
        /// <param name="documentReference">The DocumentReference to relate to.</param>
        /// <returns>Returns a new ResourceReference mapped from the provided DocumentReference.</returns>
        public static ResourceReference RelatedToDocumentReference(DocumentReference documentReference)
        {
            // masterIdentifier may not be defined, create a new identifier if this is the case
            string transactionId = documentReference.MasterIdentifier != null ? documentReference!.MasterIdentifier.Value : System.Guid.NewGuid().ToString();
            ResourceReference reference = new ResourceReference(transactionId);
            reference.Type = documentReference.GetType().Name;
            return reference;
        }
    }
}
