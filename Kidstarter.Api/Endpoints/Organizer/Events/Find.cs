using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Organizer.Events.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Models.Filters;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public class Find : EndpointBase
    {
        [HttpGet]
        [MapToApiVersion("2")]
        [Route("organizer/events/find")]
        public async Task<ApiResponse<ListDto<OrganizerEventDetailsViewModel>>> Execute(
            [FromQuery] FindEventsQuery query,
            [FromQuery] Pager pager)
        {
            var filter = EventFilter.Empty with
            {
                AgeTo = query.AgeTo,
                City = query.City,
                Directions = query.Directions ?? Array.Empty<string>(),
                EventDate = query.EventDate,
                Gender = query.Gender,
                Status = EventUserStatusFilter.All
            };

            var events = await this.Mediator.Dispatch(
                new FindEventsRequest(filter, pager.ToPagedRequest()),
                x => x.ChangeType(this.Mapper.Map<OrganizerEventDetailsViewModel[]>),
                CancellationToken.None);

            return this.ApiOk(new ListDto<OrganizerEventDetailsViewModel>(events));
        }
    }

    public class FindEventsQuery
    {
        [Range(0, 24)]
        public int? AgeTo { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        public string[] Directions { get; set; }

        [Range(typeof(DateTimeOffset), "1/1/2020", "1/1/2100")]
        public DateTimeOffset? EventDate { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }
    }
}