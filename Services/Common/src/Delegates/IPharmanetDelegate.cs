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
  public interface IPharmanetDelegate
  {
    /// <summary>
    /// The Pharmanet Delegate, which communicates directly to the Pharmanet proxy service.
    /// </summary>
    /// <param name="request">A PharmanetMessageModel instance containing the HL7v2 request message.</param>
    /// <param name="traceId">The value used to track messages from API Gateway.</param>
    /// <param name="isHealthCheck">A boolean indicating if this request is a health check or a transaction.</param>
    /// <returns>A PharmanetMessageModel as response.</returns>
    public Task<RequestResult<PharmanetMessageModel>> SubmitRequest(PharmanetMessageModel request, string traceId, bool isHealthCheck);
  }
}
