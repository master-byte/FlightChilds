using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Portfolio.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Core.Models.EF;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class CreatePortfolio : EndpointBase
    {
        [HttpPost("parent/portfolios")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<PortfolioViewModelBase>> HandleAsync(
            [FromBody] PortfolioCreateModelBase request,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Dispatch(
                             new CreatePortfolioRequest(this.Mapper.Map<PortfolioBase>(request)),
                             this.Mapper.Map<PortfolioViewModelBase>,
                             cancellationToken);

            return this.ApiCreated($"/api/v2/portfolios/filming/{result.PortfolioId}", result);
        }
    }
}