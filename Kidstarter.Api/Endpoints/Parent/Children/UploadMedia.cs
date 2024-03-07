using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models.Forms;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Exceptions;
using Kidstarter.Core.Models;
using Kidstarter.Media.Validators;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class UploadMedia : EndpointBase
    {
        [HttpPost]
        [Route("parent/children/{childId:guid}/media")]
        [MapToApiVersion("2")]
        [RequestFormLimits(ValueLengthLimit = 200_000_000, MultipartBodyLengthLimit = 200_000_000)]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<UploadViewModel[]>> HandleAsync(
            [FromRoute] Guid childId,
            [FromForm] FileUploadForm form,
            CancellationToken cancellationToken = default)
        {
            if (form.Media.Length == 0)
            {
                return new ApiResponse<UploadViewModel[]>(Array.Empty<UploadViewModel>());
            }

            var media = form.Zip();

            // ReSharper disable once PossibleNullReferenceException
            if (!media.All(mediaFile => MediaClassMatchesContentTypeValidator.IsMatch(mediaFile.MediaClass, mediaFile.File.ContentType)))
            {
                throw new BaseException("Content type/media class mismatch.");
            }

            var uploadModels = this.Mapper.Map<MediaUploadRequest[]>(media);

            var uploads = await this.Mediator.Send(
                              new UploadChildMediaFileRequest(childId, uploadModels),
                              cancellationToken);

            return this.ApiCreated(this.Mapper.Map<UploadViewModel[]>(uploads));
        }
    }
}
