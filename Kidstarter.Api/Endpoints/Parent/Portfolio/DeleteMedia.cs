using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.Children.Requests.Portfolio;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class DeleteMedia : EndpointBase
    {
        [HttpDelete("parent/portfolios/{portfolioId:guid}/media")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<string[]>> HandleAsync(
              [FromRoute] Guid portfolioId,
              DeletePortfolioMediaModel request,
              CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(
                             new DeletePortfolioMediaFileRequest(
                                 portfolioId,
                                 request.UploadIds,
                                 this.User.GetUserGuidId()),
                             cancellationToken);

            return this.ApiOk(result);
        }
    }

    public class DeletePortfolioMediaModel
    {
        public Guid[] UploadIds { get; set; }
    }
}