using System;
using System.Threading.Tasks;

using Kidstarter.Api.Endpoints.Common.Events.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Infrastructure.Extensions;
using Kidstarter.Organizations.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Events
{
    [Authorize]
    public class FindEventsByOrganization : EndpointBase
    {
        [HttpGet]
        [Route("organizations/{organizationId:guid}/events")]
        [MapToApiVersion("2")]
        public async Task<ApiResponse<ListDto<EventViewModel>>> Execute(
            [FromRoute] Guid organizationId,
            [FromQuery] Pager pager)
        {
            var result = await this.Mediator.Dispatch(
                             new FindEventsRequest(organizationId, pager.ToPagedRequest()),
                             res => new ListDto<EventViewModel>(res.ChangeType(this.Mapper.Map<EventViewModel[]>)));

            return this.ApiOk(result);
        }
    }
}
