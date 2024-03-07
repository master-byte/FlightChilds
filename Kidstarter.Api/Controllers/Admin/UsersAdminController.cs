using System;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Mappings;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Policy = Policies.Admin)]
    public class UsersAdminController : ControllerBase
    {
        private readonly IMediator mediator;

        private readonly SplitUserMapperService splitUserMapperService;

        public UsersAdminController(IMediator mediator, SplitUserMapperService splitUserMapperService)
        {
            this.mediator = mediator;
            this.splitUserMapperService = splitUserMapperService;
        }

        /// <summary>
        /// Получить детальную информацию по юзеру.
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ApiResponse<UserViewModel>> GetUserInfo(Guid userId)
        {
            var userViewModel = await this.splitUserMapperService.Map<UserViewModel>(new GetUserProfileRequest(userId));
            return this.ApiOk(userViewModel.ViewModel);
        }

        [HttpDelete]
        [Route("{userId:guid}upload/{uploadId:guid}")]
        [Authorize(Roles = "Админ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<string[]>> DeleteUpload(Guid userId, Guid uploadId)
        {
            var request = new DeleteUserMediaFileRequest(new[] { uploadId });
            var results = await this.mediator.ExecuteAs(request, new UserIdentity(userId, UserRoleTypeEnum.DoesNotMatter));

            return this.ApiOk(results);
        }
    }
}
