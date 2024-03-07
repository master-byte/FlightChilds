using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class Delete : EndpointBase
    {
        [HttpDelete("parent/children/{childId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse> HandleAsync(
            [FromRoute] Guid childId,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(new DeleteChildRequest(childId), cancellationToken);

            return this.ApiOk(true);
        }
    }
}