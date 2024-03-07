using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Users;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Parents
{
    [Authorize(Policy = Policies.Admin)]
    [Route("admin/parents")]
    public sealed class Update : EndpointBase
    {
        [HttpPut("{parentId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromRoute] Guid parentId, [FromBody] ParentUpdateModel updateModel)
        {
            var parent = new UserIdentity(parentId, UserRoleTypeEnum.Parent);

            if (updateModel.ShouldUpdateCredentials)
            {
                var result = await this.Mediator.Send(new UpdateUserCredentialsRequest(parent, updateModel.UserName, updateModel.Password));
                if (!result.Succeeded)
                {
                    return this.BadRequest(new ApiResponse<string>(result.Errors.Select(x => x.Description)));
                }
            }

            await this.Mediator.ExecuteAs(
                new UpdateUserProfileRequest(
                    p =>
                        {
                            p.UserValueName = updateModel.FullName;
                            p.PhoneNumber = updateModel.PhoneNumber;
                            p.Email = updateModel.Email;
                            p.IsActive = updateModel.IsActive;
                        }),
                parent);

            return this.NoContent();
        }
    }

    public class ParentUpdateModel
    {
        [MaxLength(250)]
        public string FullName { get; set; }

        [Phone]
        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(128)]
        public string Password { get; set; }

        [MaxLength(128)]
        public string UserName { get; set; }

        public bool IsActive { get; set; }

        public bool ShouldUpdateCredentials => !string.IsNullOrEmpty(this.Password) || !string.IsNullOrEmpty(this.UserName);
    }
}