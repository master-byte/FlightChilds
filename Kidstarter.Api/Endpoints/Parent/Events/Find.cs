using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Events.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Parent.Events;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Models.Filters;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Events
{
    [Authorize(Policy = Policies.Parent)]
    public class Find : EndpointBase
    {
        [HttpGet]
        [MapToApiVersion("2")]
        [Route("parent/events/find")]
        public async Task<ApiResponse<ListDto<ParentEventDetailsViewModel>>> Execute(
            [FromQuery] FindEventsQuery query,
            [FromQuery] Pager pager)
        {
            var tags = (query.SearchTags ?? string.Empty)
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Take(3)
                .ToArray();

            var filter = new EventFilter(
                query.AgeTo,
                query.City?.Trim(),
                query.Directions ?? Array.Empty<string>(),
                query.EventDate,
                query.Gender,
                query.Status,
                query.OrganizationId,
                query.SearchString?.Trim(),
                tags);

            var events = await this.Mediator.Dispatch(
                new FindEventsRequest(filter, pager.ToPagedRequest(), query.OrderBy),
                res => res.ChangeType(this.Mapper.Map<ParentEventDetailsViewModel[]>));

            return this.ApiOk(new ListDto<ParentEventDetailsViewModel>(events));
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

        [EnumDataType(typeof(EventUserStatusFilter))]
        public EventUserStatusFilter Status { get; set; }

        public Guid? OrganizationId { get; set; }

        [FromQuery(Name = "q")]
        [MaxLength(30)]
        public string SearchString { get; set; }

        [FromQuery(Name = "t")]
        [MaxLength(100)]
        public string SearchTags { get; set; }
    }
}