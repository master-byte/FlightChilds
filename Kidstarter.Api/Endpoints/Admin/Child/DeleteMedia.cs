using System;
using System.Linq;
using System.Threading;
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
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Admin)]
    public class DeleteMedia : EndpointBase
    {
        private readonly IEntityService entityService;

        public DeleteMedia(IEntityService entityService)
        {
            this.entityService = entityService;
        }

        [HttpDelete("admin/children/{childId:guid}/media")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> HandleAsync(
            [FromRoute] Guid childId,
            [FromBody] DeleteChildMediaModel request,
            CancellationToken cancellationToken = default)
        {
            var parentId = await this.entityService.NoTrackingSet<Core.Models.EF.Child>()
                               .Where(x => x.ChildId == childId)
                               .Select(x => x.ParentId)
                               .SingleOrDefaultAsync(cancellationToken);

            if (parentId == null)
            {
                return this.NotFound(new ApiResponse<string>("Родитель с указанным ID не найден."));
            }

            await this.Mediator.ExecuteAs(
                new DeleteChildMediaFileRequest(childId, request.UploadIds),
                new(parentId, UserRoleTypeEnum.Parent),
                cancellationToken);

            return this.NoContent();
        }
    }

    public class DeleteChildMediaModel
    {
        public Guid[] UploadIds { get; set; }
    }
}