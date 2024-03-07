using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Organizations;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Organizations
{
    [Authorize(Policy = Policies.Parent)]
    public class GetFavourite : EndpointBase
    {
        [HttpGet]
        [Route("parent/organizations/favourite")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<PagedResult<OrganizationViewModel>>> HandleAsync(
            [FromQuery] Pager pager,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(
                new GetFavouriteOrganizationsRequest(pager.ToPagedRequest()),
                cancellationToken);

            var orgs = result.ChangeType(this.Mapper.Map<OrganizationViewModel[]>);

            return this.ApiOk(orgs);
        }
    }
}