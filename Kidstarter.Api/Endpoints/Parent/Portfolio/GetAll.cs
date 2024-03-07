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
    public class GetAll : EndpointBase
    {
        [HttpGet("parent/children/{childId:guid}/portfolios")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<PortfolioViewModelBase[]>> HandleAsync(
            Guid childId,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Dispatch(
                       new GetPortfoliosRequest(childId),
                       this.Mapper.Map<PortfolioViewModelBase[]>,
                       cancellationToken);

            return this.ApiOk(result);
        }
    }
}