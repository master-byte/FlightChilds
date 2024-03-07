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
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Entities
{
    public enum InteractionType
    {
        View,

        Subscribe,

        Unsubscribe,

        Like,
    }

    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class Actions : EndpointBase
    {
        [HttpPost("parent/entities/{entityExternalId:guid}/interactions")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Execute(
            [FromRoute] Guid entityExternalId,
            [FromBody] InteractionModel action,
            CancellationToken cancellationToken = default)
        {
            IRequest<Unit> request = action.Type switch
            {
                InteractionType.View => new ViewEntityRequest(Identity.Create(entityExternalId)),
                //// InteractionType.Subscribe => new SubscribeToEventRequest(entityExternalId, action.PortfolioId, this.GetCurrentUser()),
                InteractionType.Unsubscribe => new UnsubscribeFromEventRequest(entityExternalId),
                InteractionType.Like => new ChangeEntityFavouriteStatusRequest(Identity.Create(entityExternalId)),
                _ => throw new ArgumentOutOfRangeException(nameof(action.Type))
            };

            await this.Mediator.Send(request, cancellationToken);

            return this.NoContent();
        }
    }

    public class InteractionModel
    {
        [EnumDataType(typeof(InteractionType))]
        public InteractionType Type { get; set; }
    }
}
