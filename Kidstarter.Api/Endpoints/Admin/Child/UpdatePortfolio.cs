using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Portfolio.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Services;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Admin)]
    public class UpdatePortfolio : EndpointBase
    {
        private readonly IEntityService entityService;

        public UpdatePortfolio(IEntityService entityService)
        {
            this.entityService = entityService;
        }

        [HttpPatch("admin/portfolios/{portfolioId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> HandleAsync(
            [FromRoute] Guid portfolioId,
            [FromBody] FilmingPortfolioUpdateModel request,
            CancellationToken cancellationToken = default)
        {
            var parentId = await this.entityService.NoTrackingSet<PortfolioBase>()
                               .Where(x => x.PortfolioId == portfolioId)
                               .Select(x => x.Child.ParentId)
                               .SingleOrDefaultAsync(cancellationToken);

            if (parentId == null)
            {
                return this.NotFound(new ApiResponse<string>("Родитель с указанным ID не найден."));
            }

            await this.Mediator.ExecuteAs(
                new UpdatePortfolioRequest(portfolioId, p => this.Mapper.Map(request, p)),
                new(parentId, UserRoleTypeEnum.Parent),
                cancellationToken);

            return this.Ok();
        }
    }
}