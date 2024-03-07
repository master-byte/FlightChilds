using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Events.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.Filters;
using Kidstarter.Events.Models;
using Kidstarter.Events.Requests.Parent;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Events
{
    [Authorize(Policy = Policies.Parent)]
    public class Invitations : EndpointBase
    {
        [HttpGet]
        [MapToApiVersion("2")]
        [Route("parent/events/invitations")]
        public async Task<ApiResponse<ListDto<ParentEventDetailsViewModel>>> Execute([FromQuery] Pager pager)
        {
            var filter = new EventFilter
            {
                Status = EventUserStatusFilter.Invited
            };

            var events = await this.Mediator.Dispatch(
                new FindEventsRequest(filter, pager.ToPagedRequest(), EventOrdering.EventDateAsc),
                res =>
                {
                    var mapped = res.ChangeType(this.Mapper.Map<ParentEventDetailsViewModel[]>);
                    return new ListDto<ParentEventDetailsViewModel>(mapped);
                });

            return this.ApiOk(events);
        }
    }
}
