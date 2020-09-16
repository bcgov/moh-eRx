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
namespace Health.PharmaNet.Delegates
{
  using System.Threading.Tasks;

  using Health.PharmaNet.Models;

  /// <summary>
  /// The Pharmanet Delegate, which communicates directly to the Pharmanet proxy service.
  /// </summary>
  public class PharmanetDelegate : IPharmanetDelegate
  {
    /// <summary>
    /// Submit a PharmanetMessage to Pharmanet System.
    /// </summary>
    /// <param name="request">The PharmanetMessage request containing HL7v2 base 64 payload.</param>
    /// <returns>A PharmanetMessage response.</returns>
    public async Task<PharmanetMessage> SubmitRequest(PharmanetMessage request)
    {
      PharmanetMessage response = new PharmanetMessage();

      // 0. Setup authentication.
      // 1. Submit the request to the Pharmanet HL7v2 protected endpoint.
      // 2. Return the response.
      return response;
    }
  }
}
