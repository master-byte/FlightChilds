using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.EventSubscriptions
{
    public enum SubscriptionAction
    {
        View,

        Reject,

        Invite
    }

    [VersionedEndpoint]
    [Authorize(Policy = Policies.Organizer)]
    public class SubscriptionActions : ControllerBase
    {
        private readonly IMediator mediator;

        public SubscriptionActions(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("organizer/eventSubscriptions/{subscriptionId:guid}/actions")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<bool>> HandleAsync(
            [FromRoute] Guid subscriptionId,
            [FromBody] PerformEventAction action,
            CancellationToken cancellationToken = default)
        {
            IRequest<Unit> request = action.Action switch
            {
                SubscriptionAction.View => new ViewEventSubscriptionRequest(subscriptionId),
                SubscriptionAction.Reject => new RejectEventSubscriptionRequest(subscriptionId),
                SubscriptionAction.Invite => new InviteToEventRequest(subscriptionId),
                _ => throw new ArgumentOutOfRangeException()
            };

            await this.mediator.Send(request, cancellationToken);

            return this.ApiOk(true);
        }
    }

    public class PerformEventAction
    {
        [EnumDataType(typeof(SubscriptionAction))]
        public SubscriptionAction Action { get; set; }
    }
}