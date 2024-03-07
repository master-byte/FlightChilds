using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public sealed class DeleteMedia : EndpointBase
    {
        [HttpDelete]
        [Route("organizer/events/{eventId:guid}/uploads")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse> HandleAsync(
            [FromRoute] Guid eventId,
            [FromBody] Guid[] uploadIds,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(
                new DeleteEventMediaFileRequest(eventId, uploadIds.Select(Identity.Create).ToArray()),
                cancellationToken);

            return this.ApiStatusCode(HttpStatusCode.OK);
        }
    }
}