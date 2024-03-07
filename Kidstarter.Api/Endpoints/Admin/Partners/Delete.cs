using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Partners
{
    [Authorize(Policy = Policies.Admin)]
    public sealed class Delete : EndpointBase
    {
        [HttpDelete("admin/partners/{partnerId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> HandleAsync([FromRoute] string partnerId, CancellationToken cancellationToken = default)
        {
            var partner = new UserIdentity(partnerId, UserRoleTypeEnum.Partner);

            await this.Mediator.ExecuteAs(
                new UpdateUserProfileRequest(x => x.IsActive = false),
                partner,
                cancellationToken);

            return this.NoContent();
        }
    }
}