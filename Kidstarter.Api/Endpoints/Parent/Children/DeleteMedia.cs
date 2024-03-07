using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class DeleteMedia : EndpointBase
    {
        [HttpDelete("parent/children/{childId:guid}/media")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<string[]>> HandleAsync(
            [FromRoute] Guid childId,
            [FromBody] DeleteChildMediaModel request,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(
                             new DeleteChildMediaFileRequest(childId, request.UploadIds),
                             cancellationToken);

            return this.ApiOk(result);
        }
    }

    public class DeleteChildMediaModel
    {
        public Guid[] UploadIds { get; set; }
    }
}