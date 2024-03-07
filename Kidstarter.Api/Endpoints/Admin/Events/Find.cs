using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Admin.Events.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Admin.Events;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Events
{
    [Authorize(Policy = Policies.Admin)]
    [Route("admin/events")]
    public class Find : EndpointBase
    {
        [HttpGet("find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Execute(
            [FromQuery] FindEventsQuery query,
            [FromQuery] PagerConstrained pager)
        {
            var tags = (query.SearchTags ?? string.Empty)
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Take(3)
                .ToArray();

            var events = await this.Mediator.Send(
                             new FindEventsRequest(
                                 query.AgeTo,
                                 query.City?.Trim(),
                                 query.Directions ?? Array.Empty<string>(),
                                 query.EventDate,
                                 query.Gender,
                                 query.OrderBy,
                                 query.OrganizationId,
                                 query.SearchString?.Trim(),
                                 tags,
                                 pager.ToPagedRequest()));

            return this.Ok(
                events.ChangeType(
                    e => e.Select(
                        x => new EventViewModel
                        {
                            EntityId = x.Event.EntityId,
                            IsActive = x.Event.IsActive,
                            EventDate = x.Event.EventDate,
                            EventId = x.Event.EventId,
                            Name = x.Event.Name,
                            EventCategory = x.Event.Direction.Name,
                            EventType = x.Event.Direction.Parent?.Name,
                        })));
        }
    }

    public class FindEventsQuery
    {
        [Range(0, 24)]
        public int? AgeTo { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        public string[] Directions { get; set; }

        [Range(typeof(DateTime), "1/1/2020", "1/1/2100")]
        public DateTime? EventDate { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [EnumDataType(typeof(EventOrdering))]
        public EventOrdering OrderBy { get; set; }

        public Guid? OrganizationId { get; set; }

        [FromQuery(Name = "q")]
        [MaxLength(30)]
        public string SearchString { get; set; }

        [FromQuery(Name = "t")]
        [MaxLength(100)]
        public string SearchTags { get; set; }
    }
}