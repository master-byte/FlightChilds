using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Interests
{
    [Authorize(Policy = Policies.Parent)]
    public class GetAll : EndpointBase
    {
        [HttpGet("parent/interests/all")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<InterestViewModel[]>> Execute(CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(new GetAllInterestsRequest(), cancellationToken);

            return this.ApiOk(
                result.Select(
                    x => new InterestViewModel
                    {
                        Name = x.Name,
                        EventDirectionName = x.EventDirection.Name,
                        InterestId = x.InterestId,
                        PlanetLogoUrlX2 = x.PlanetLogoUrlX2,
                        PlanetLogoUrlX4 = x.PlanetLogoUrlX4
                    }).ToArray());
        }
    }
}