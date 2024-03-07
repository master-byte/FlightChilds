using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Interests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Interests
{
    [Authorize(Policy = Policies.Parent)]
    public class Add : EndpointBase
    {
        [HttpPost]
        [Route("parent/interests")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<bool>> Execute([FromBody] InterestsModel interestsModel, CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(
                new AddUserInterestsRequest(interestsModel.InterestIds),
                cancellationToken);

            return this.ApiOk(true);
        }
    }

    public sealed class InterestsModel
    {
        public string[] InterestIds { get; set; }
    }
}