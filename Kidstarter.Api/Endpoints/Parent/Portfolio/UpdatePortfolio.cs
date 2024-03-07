using System;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Portfolio.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class UpdatePortfolio : EndpointBase
    {
        [HttpPatch("parent/filmingPortfolios/{portfolioId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<PortfolioViewModelBase>> HandleAsync(
            [FromRoute] Guid portfolioId,
            [FromBody] FilmingPortfolioUpdateModel request,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Dispatch(
                             new UpdatePortfolioRequest(portfolioId, p => this.Mapper.Map(request, p)),
                             this.Mapper.Map<PortfolioViewModelBase>,
                             cancellationToken);

            return this.ApiOk(result);
        }
    }
}