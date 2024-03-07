using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class DeletePortfolio : EndpointBase
    {
        [HttpDelete("parent/portfolios/{portfolioId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ApiResponse> HandleAsync(
            [FromRoute] Guid portfolioId,
            CancellationToken cancellationToken = default)
        {
            await this.Mediator.Send(
                new DeletePortfolioRequest(portfolioId),
                cancellationToken);

            return this.ApiStatusCode(HttpStatusCode.NoContent);
        }
    }
}