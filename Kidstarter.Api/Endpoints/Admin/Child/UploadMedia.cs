using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models.Forms;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Exceptions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Services;
using Kidstarter.Infrastructure.Extensions;
using Kidstarter.Media.Validators;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Admin)]
    public class UploadMedia : EndpointBase
    {
        private readonly IEntityService entityService;

        public UploadMedia(IEntityService entityService)
        {
            this.entityService = entityService;
        }

        [HttpPost]
        [Route("admin/children/{childId:guid}/media")]
        [MapToApiVersion("2")]
        [RequestFormLimits(ValueLengthLimit = 200_000_000, MultipartBodyLengthLimit = 200_000_000)]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> HandleAsync(
            [FromRoute] Guid childId,
            [FromForm] FileUploadForm form,
            CancellationToken cancellationToken = default)
        {
            if (form.Media.Length == 0)
            {
                return this.Ok(Enumerable.Empty<string>());
            }

            var media = form.Zip();

            if (!media.All(mediaFile => MediaClassMatchesContentTypeValidator.IsMatch(mediaFile.MediaClass, mediaFile.File.ContentType)))
            {
                throw new BaseException("Content type/media class mismatch.");
            }

            var parentId = await this.entityService.NoTrackingSet<Core.Models.EF.Child>()
                               .Where(x => x.ChildId == childId)
                               .Select(x => x.ParentId)
                               .SingleOrDefaultAsync(cancellationToken);

            if (parentId == null)
            {
                return this.NotFound(new ApiResponse<string>("Родитель с указанным ID не найден."));
            }

            var uploadModels = media.Select(
                x => new MediaUploadRequest
                {
                    ContentType = x.File.ContentType,
                    FileStream = x.File.OpenReadStream(),
                    MediaClass = x.MediaClass,
                    OriginalFileName = x.File.FileName,
                }).ToArray();

            await this.Mediator.ExecuteAs(
                new UploadChildMediaFileRequest(childId, uploadModels),
                new(parentId, UserRoleTypeEnum.Parent),
                cancellationToken);

            return this.Ok();
        }
    }
}
