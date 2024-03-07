using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Organizations
{
    [Authorize]
    public class Find : EndpointBase
    {
        [HttpGet]
        [MapToApiVersion("2")]
        [Route("organizations/all")]
        [Route("organizations/find")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<ListDto<OrganizationViewModel>>> Execute(
            [FromQuery] FindOrganizationsQuery query,
            [FromQuery] Pager pager)
        {
            var result = await this.Mediator.Send(
                             new GetOrganizationsRequest(
                                 query.Directions,
                                 query.SearchString,
                                 query.Age,
                                 query.City,
                                 query.DaysOfWeek?.Distinct().Take(7).ToArray(),
                                 pager.ToPagedRequest()));

            var viewModels = result.ChangeType(o => this.Mapper.Map<OrganizationViewModel[]>(o));

            if (query.Directions != null)
            {
                foreach (var org in viewModels.Entities)
                {
                    org.Directions = org.Directions.OrderByDescending(x => Array.IndexOf(query.Directions, x.EventDirectionId));
                }
            }

            return this.ApiOk(new ListDto<OrganizationViewModel>(viewModels));
        }

        public class FindOrganizationsQuery
        {
            public string[] Directions { get; set; }

            [FromQuery(Name = "q")]
            [MaxLength(30)]
            public string SearchString { get; set; }

            [Range(0, 24)]
            public int? Age { get; set; }

            [MaxLength(100)]
            public string City { get; set; }

            public DayOfWeek[] DaysOfWeek { get; set; }
        }
    }
}