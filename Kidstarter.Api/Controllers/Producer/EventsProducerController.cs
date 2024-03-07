using System;
using System.Threading.Tasks;

using AutoMapper;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Events;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Filters;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers.Producer
{
    [Route("api/producer/events")]
    [ApiController]
    [Authorize(Policy = Policies.Producer)]
    public class EventsProducerController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public EventsProducerController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        [HttpGet("{eventId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<CastingViewModelV1>> Get(Guid eventId)
        {
            var @event = await this.mediator.Send(new GetEventRequest(eventId));
            return @event == null
                       ? this.ApiNotFound<CastingViewModelV1>()
                       : this.ApiOk(this.mapper.Map<CastingViewModelV1>(@event.Event as Casting));
        }

        [HttpGet("list")]
        public async Task<ApiResponse<ListDto<CastingViewModelV1>>> GetAllEvents([FromQuery] Pager pager)
        {
            var result = await this.mediator.Send(new FindEventsRequest(EventFilter.Empty, pager.ToPagedRequest()));
            var eventDetails = result.ChangeType(this.mapper.Map<CastingViewModelV1[]>);

            return this.ApiOk(new ListDto<CastingViewModelV1>(eventDetails));
        }
    }
}