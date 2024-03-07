using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent;
using Kidstarter.Core.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Organizations
{
    [Authorize(Policy = Policies.Parent)]
    public class ChangeIsFavouriteStatus : EndpointBase
    {
        [HttpPost]
        [Route("parent/organizations/{organizationId:guid}/favourite")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<bool>> HandleAsync(
            [FromRoute] Guid organizationId,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(
                new ChangeEntityFavouriteStatusRequest(Identity.Create(organizationId)),
                cancellationToken);

            return this.ApiOk(true);
        }
    }
}