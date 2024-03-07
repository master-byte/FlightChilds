using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public sealed class Delete : EndpointBase
    {
        [HttpDelete]
        [Route("organizer/events/{eventId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse> HandleAsync(
            [FromRoute] Guid eventId,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(new DeleteEventRequest(eventId), cancellationToken);

            return this.ApiOk(true);
        }
    }
}