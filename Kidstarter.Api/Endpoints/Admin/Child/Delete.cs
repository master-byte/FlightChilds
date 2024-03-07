using System;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Services;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [Authorize(Policy = Policies.Admin)]
    public class Delete : EndpointBase
    {
        private readonly IEntityService entityService;

        public Delete(IEntityService entityService)
        {
            this.entityService = entityService;
        }

        [HttpDelete("admin/children/{childId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization([FromRoute] Guid childId)
        {
            var parentId = await this.entityService.NoTrackingSet<Core.Models.EF.Child>()
                               .Where(x => x.ChildId == childId)
                               .Select(x => x.ParentId)
                               .SingleOrDefaultAsync();

            if (parentId == null)
            {
                return this.NotFound();
            }

            await this.Mediator.ExecuteAs(
                new DeleteChildRequest(childId),
                new(parentId, UserRoleTypeEnum.Parent));

            return this.NoContent();
        }
    }
}
