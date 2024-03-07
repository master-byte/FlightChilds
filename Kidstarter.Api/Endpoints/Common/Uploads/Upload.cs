using System.Threading.Tasks;

using Kidstarter.Api.Models.Forms;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Uploads
{
    [Authorize]
    public class Upload : EndpointBase
    {
        [HttpPost]
        [Route("uploads")]
        [MapToApiVersion("2")]
        [RequestFormLimits(ValueLengthLimit = 200_000_000, MultipartBodyLengthLimit = 200_000_000)]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<UploadWithoutBindingResponse[]>> Execute([FromForm] FileUploadForm uploadForm)
        {
            if (uploadForm?.Media == null || uploadForm.Media.Length == 0)
            {
                return this.ApiError<UploadWithoutBindingResponse[]>("Noting to upload.");
            }

            var result = await this.Mediator.Send(
                             new UploadWithoutBindingRequest(
                                 this.Mapper.Map<MediaUploadRequest[]>(uploadForm.Zip())));

            return this.ApiOk(result);
        }
    }
}