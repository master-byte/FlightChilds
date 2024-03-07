using System;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public class Get : EndpointBase
    {
        [HttpGet("admin/organizations/{organizationId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromRoute] Guid organizationId)
        {
            var org = await this.Mediator.Send(new GetOrganizationRequest(organizationId));

            return this.Ok(org.Organization.ToViewModel(org.IsFavourite));
        }
    }
}