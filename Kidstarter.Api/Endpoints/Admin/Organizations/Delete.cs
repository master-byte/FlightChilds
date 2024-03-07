using System;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public class Delete : EndpointBase
    {
        [HttpDelete("admin/organizations/{organizationId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization([FromRoute] Guid organizationId)
        {
            await this.Mediator.Send(
                new UpdateOrganizationRequest(organizationId, x => x.IsActive = false, null, null));

            return this.NoContent();
        }
    }
}