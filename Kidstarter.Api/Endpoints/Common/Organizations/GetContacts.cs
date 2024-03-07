using System;
using System.Threading.Tasks;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Organizations
{
    [Authorize]
    public class GetContacts : EndpointBase
    {
        [HttpGet("organizations/{organizationId:guid}/contacts")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<OrganizationContactsViewModel>> Get([FromRoute] Guid organizationId)
        {
            var result = await this.Mediator.Dispatch(
                new GetOrganizationContactsRequest(organizationId),
                this.Mapper.Map<OrganizationContactsViewModel>);

            return this.ApiOk(result);
        }
    }

    public class OrganizationContactsViewModel
    {
        public string PhoneNumber { get; set; }

        public BusinessHoursViewModel[] BusinessHours { get; set; }
    }
}
