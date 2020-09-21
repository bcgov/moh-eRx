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
namespace Health.PharmaNet.Common.Authorization
{
    /// <summary>
    /// object to pass as the TResource type for the handler.
    /// </summary>
    public class MessageType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageType"/> class.
        /// </summary>
        /// <param name="messageType">The value to set.</param>
        public MessageType(string messageType)
        {
            this.Value = messageType;
        }

        /// <summary>
        /// Gets or sets the MessageType value.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}