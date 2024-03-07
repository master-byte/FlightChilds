using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models.Forms;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Models;
using Kidstarter.Infrastructure.Extensions;
using Kidstarter.Media.Validators;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Organizer.Events
{
    [Authorize(Policy = Policies.Organizer)]
    public sealed class UploadMedia : EndpointBase
    {
        [HttpPost]
        [Route("organizer/events/{eventId:guid}/uploads")]
        [MapToApiVersion("2")]
        [RequestFormLimits(ValueLengthLimit = 200_000_000, MultipartBodyLengthLimit = 200_000_000)]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse> HandleAsync(
            [FromRoute] Guid eventId,
            [FromForm] FileUploadForm uploadForm,
            CancellationToken cancellationToken = default)
        {
            if (uploadForm?.Media == null || uploadForm.Media.Length == 0)
            {
                return this.ApiError("Noting to upload.");
            }

            var media = uploadForm.Zip();

            // ReSharper disable once PossibleNullReferenceException
            if (!media.All(mediaFile => MediaClassMatchesContentTypeValidator.IsMatch(mediaFile.MediaClass, mediaFile.File.ContentType)))
            {
                return this.ApiError("Content type/media class mismatch.");
            }

            var uploadModels = this.Mapper.Map<MediaUploadRequest[]>(media);

            var uploads = await this.Mediator.Dispatch(
                                     new UploadEventMediaFileRequest(eventId, uploadModels),
                                     this.Mapper.Map<UploadViewModel[]>,
                                     cancellationToken);

            return this.ApiOk(uploads);
        }
    }
}
