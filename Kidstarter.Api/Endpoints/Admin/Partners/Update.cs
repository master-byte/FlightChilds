using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Partners;
using Kidstarter.BusinessLogic.Requests.Admin.Users;
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
    public sealed class Update : EndpointBase
    {
        [HttpPut("admin/partners/{partnerId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> HandleAsync(
            [FromRoute] string partnerId,
            [FromBody] PartnerModel updateModel,
            CancellationToken cancellationToken = default)
        {
            var partner = new UserIdentity(partnerId, UserRoleTypeEnum.Partner);

            if (updateModel.ShouldUpdateCredentials)
            {
                var result = await this.Mediator.Send(
                                 new UpdateUserCredentialsRequest(partner, updateModel.UserName, updateModel.Password),
                                 cancellationToken);

                if (!result.Succeeded)
                {
                    return this.BadRequest(new ApiResponse<string>(result.Errors.Select(x => x.Description)));
                }
            }

            await this.Mediator.ExecuteAs(
                new UpdateUserProfileRequest(
                    p =>
                        {
                            p.PhoneNumber = updateModel.PhoneNumber;
                            p.Email = updateModel.Email;
                            p.IsActive = updateModel.IsActive;
                        }),
                partner,
                cancellationToken);

            await this.Mediator.Send(
                new UpdatePartnerRequest(
                    partnerId,
                    p =>
                        {
                            p.FirstName = updateModel.FirstName;
                            p.SecondName = updateModel.SecondName;
                            p.Status = updateModel.Status;
                        }),
                cancellationToken);

            return this.NoContent();
        }

        public sealed class PartnerModel
        {
            [Required(AllowEmptyStrings = false)]
            [MaxLength(100)]
            public string UserName { get; set; }

            [Required(AllowEmptyStrings = false)]
            [MaxLength(128)]
            public string Password { get; set; }

            [Required(AllowEmptyStrings = false)]
            [MaxLength(60)]
            public string FirstName { get; set; }

            [Required(AllowEmptyStrings = false)]
            [MaxLength(60)]
            public string SecondName { get; set; }

            [Required(AllowEmptyStrings = false)]
            [Phone]
            [MaxLength(16)]
            public string PhoneNumber { get; set; }

            [Required(AllowEmptyStrings = false)]
            [EmailAddress]
            [MaxLength(200)]
            public string Email { get; set; }

            [EnumDataType(typeof(PartnerStatus))]
            public PartnerStatus Status { get; set; }

            public bool IsActive { get; set; }

            public bool ShouldUpdateCredentials => !string.IsNullOrEmpty(this.Password) || !string.IsNullOrEmpty(this.UserName);
        }
    }
}