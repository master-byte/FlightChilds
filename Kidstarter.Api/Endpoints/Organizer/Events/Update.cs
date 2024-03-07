using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Update;
using Kidstarter.Api.Models.View.Events;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Models.EF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public sealed class Update : EndpointBase
    {
        [HttpPatch]
        [Route("organizer/events/{eventId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<EventViewModel>> HandleAsync(
            [FromRoute] Guid eventId,
            [FromBody] EventUpdateModelBase updateModel,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(
                new UpdateEventRequest<Event>(eventId, x => this.Mapper.Map(updateModel, x)),
                cancellationToken);

            return this.ApiOk(
                new EventViewModel
                {
                    EventId = eventId
                });
        }
    }
}
