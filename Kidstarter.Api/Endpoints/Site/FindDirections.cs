using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Site
{
    [AllowAnonymous]
    public class FindDirections : EndpointBase
    {
        [HttpGet]
        [Route("site/eventDirections/find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDirectionTreeNode[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromQuery] GetEventDirectionsQuery query)
        {
            var result = await this.Mediator.Send(
                             new GetDirectionsHierarchyRequest(
                                 query.StartFrom,
                                 query.LevelFrom,
                                 query.LevelTo,
                                 query.Take,
                                 query.OrderBy));

            return this.Ok(result);
        }

        public class GetEventDirectionsQuery
        {
            public string StartFrom { get; set; }

            [Range(1, 10)]
            public uint LevelFrom { get; set; } = 1;

            [Range(1, 10)]
            public uint LevelTo { get; set; } = 10;

            [EnumDataType(typeof(SortDirection))]
            public SortDirection OrderBy { get; set; }

            [Range(1, int.MaxValue)]
            public uint Take { get; set; } = int.MaxValue;
        }
    }
}
