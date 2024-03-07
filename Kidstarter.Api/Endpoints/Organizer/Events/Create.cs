using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Create;
using Kidstarter.Api.Models.View.Events;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Models.EF;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public sealed class Create : EndpointBase
    {
        [HttpPost]
        [Route("organizer/events")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<EventViewModel>> HandleAsync(
            [FromBody] EventCreateModelBase createModel,
            CancellationToken cancellationToken = default)
        {
            var createdEvent = await this.Mediator.Dispatch(
                             new CreateEventRequest(
                                 this.Mapper.Map<Event>(createModel, opt => opt.Items["CreatorId"] = this.GetCurrentUser().Id),
                                 createModel.CategoryId),
                             this.Mapper.Map<EventViewModel>,
                             cancellationToken);

            return this.ApiCreated($"api/v2/organizer/events/{createdEvent.EventId}", createdEvent);
        }
    }
}
