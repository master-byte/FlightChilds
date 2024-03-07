using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Admin)]
    public class FindByParent : EndpointBase
    {
        [HttpGet("admin/children/by-parent/{parentId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> HandleAsync([FromRoute] Guid parentId, CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.ExecuteAs(
                       new GetChildrenRequest(),
                       new(parentId, UserRoleTypeEnum.Parent),
                       cancellationToken);

            return this.Ok(result.Select(x => x.ToViewModel()));
        }
    }
}