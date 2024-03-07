using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Children.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class GetAll : EndpointBase
    {
        [HttpGet("parent/children")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<ChildViewModel[]>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(
                       new GetChildrenRequest(),
                       cancellationToken);

            return this.ApiOk(result.Select(x => x.ToViewModel()).ToArray());
        }
    }
}