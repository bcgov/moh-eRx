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
namespace Health.PharmaNet.Authorization.Requirements.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class representing a segment field signature for a HL7v2 message evaluation.
    /// </summary>
    public class SegmentField
    {
        /// <summary>Gets or sets the SegmentField Index. Set to -1 to match any segment.</summary>
        [JsonPropertyName("Index")]
        public int Index { get; set; } = -1;

        /// <summary>Gets or sets the SegmentField Value.</summary>
        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>Gets or sets whether the SegmentField Value is exact match, or startsWith.</summary>
        public MatchType? ValueMatchType { get; set; } = MatchType.Exact;
    }

    /// <summary>Choice of match types for the value in the segment.</summary>
    public enum MatchType { 
        ///<summary>When an exact match in the segment is required.</summary>
        Exact, 
        ///<summary>When the value must be contained (found) in the segment is required.</summary>
        Contains, 
         ///<summary>When the segment must start with the Value is required.</summary>
       StartsWith 
    }
}