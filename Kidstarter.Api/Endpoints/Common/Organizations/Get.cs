using System;
using System.Threading.Tasks;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Organizations
{
    [Authorize]
    public class Get : EndpointBase
    {
        [HttpGet]
        [Route("organizations/{organizationId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<OrganizationViewModel>> Execute(Guid organizationId)
        {
            var org = await this.Mediator.Send(new GetOrganizationRequest(organizationId));

            return this.ApiOk(org.Organization.ToViewModel(org.IsFavourite));
        }
    }
}