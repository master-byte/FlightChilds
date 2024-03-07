using System;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Admin.Organizations;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers
{
    [Route("api/producercenter")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public OrganizationsController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        [HttpGet("{producerCenterId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("Use corresponding Endpoint - Get")]
        public async Task<ApiResponse<OrganizationViewModel>> Get(Guid producerCenterId)
        {
            var result = await this.mediator.Send(new GetOrganizationRequest(producerCenterId));
            if (result == null)
            {
                return this.ApiNotFound<OrganizationViewModel>();
            }

            return this.ApiOk(this.mapper.Map<OrganizationViewModel>(result));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ApiResponse<OrganizationViewModel>> Create([FromBody] OrganizationCreateModel createModel)
        {
            var result = await this.mediator.Send(
                             new CreateOrganizationRequest(
                                 this.mapper.Map<Organization>(createModel)));

            return this.ApiCreated(this.mapper.Map<OrganizationViewModel>(result));
        }

        [HttpPost]
        [Route("{producerCenterId:guid}/upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize(Roles = "Продюсер,Админ")]
        public async Task<ApiResponse<UploadViewModel[]>> Upload(Guid producerCenterId, IFormFile[] media)
        {
            if (media.Length == 0)
            {
                return new ApiResponse<UploadViewModel[]>(Array.Empty<UploadViewModel>());
            }

            var uploads = await this.mediator.Send(
                              new UploadOrganizationMediaFileRequest(
                                  producerCenterId,
                                  this.mapper.Map<MediaUploadRequest[]>(media)));

            return this.ApiCreated(this.mapper.Map<UploadViewModel[]>(uploads));
        }

        [HttpDelete]
        [Route("{producerCenterId:guid}/upload/{uploadId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Продюсер,Админ")]
        public async Task<ApiResponse<string[]>> DeleteUpload(Guid producerCenterId, Guid uploadId)
        {
            var results = await this.mediator.Send(
                              new DeleteOrganizationMediaFileRequest(
                                  producerCenterId,
                                  new[] { uploadId }));

            return this.ApiOk(results);
        }
    }
}