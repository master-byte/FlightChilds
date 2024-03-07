using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Events.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Events;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Events
{
    [Authorize(Policy = Policies.Parent)]
    public sealed class Get : EndpointBase
    {
        [HttpGet]
        [Route("parent/events/{eventId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<ParentEventDetailsViewModel>> HandleAsync(
            [FromRoute] Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var eventViewModel = await this.Mediator.Dispatch(
                             new GetEventRequest(eventId),
                             res => this.Mapper.Map<ParentEventDetailsViewModel>(res),
                             cancellationToken);

            return this.ApiOk(eventViewModel);
        }
    }
}
