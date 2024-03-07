using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent;
using Kidstarter.BusinessLogic.Requests.Parent.Events;
using Kidstarter.Core.Identity;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Events
{
    public enum EventAction
    {
        View,

        Subscribe,

        Unsubscribe,
    }

    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    [Obsolete("Use entities/actions")]
    public class EventActions : EndpointBase
    {
        [HttpPost("parent/events/{eventId:guid}/actions")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<bool>> HandleAsync(
            [FromRoute] Guid eventId,
            [FromBody] PerformEventAction action,
            CancellationToken cancellationToken = default)
        {
            IRequest<Unit> request = action.Action switch
            {
                EventAction.View => new ViewEntityRequest(Identity.Create(eventId)),
                EventAction.Subscribe => new SubscribeToEventRequest(eventId, action.PortfolioId),
                EventAction.Unsubscribe => new UnsubscribeFromEventRequest(eventId),
                _ => throw new ArgumentOutOfRangeException()
            };

            await this.Mediator.Send(request, cancellationToken);

            return this.ApiOk(true);
        }
    }

    public class PerformEventAction
    {
        [EnumDataType(typeof(EventAction))]
        public EventAction Action { get; set; }

        public Guid PortfolioId { get; set; }
    }
}