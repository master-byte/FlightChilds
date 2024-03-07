using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models;
using Kidstarter.BusinessLogic.Requests.Admin.Partners;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Partners
{
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = Policies.Admin)]
    public class Create : EndpointBase
    {
        [HttpPost("admin/partners")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
        public async Task<IActionResult> CreatePartner([FromBody] PartnerModel model)
        {
            var (user, result) = await this.Mediator.Send(
                                       new RegisterUserRequest(
                                           model.UserName,
                                           model.Password,
                                           model.PhoneNumber,
                                           model.Email,
                                           model.IsActive,
                                           null,
                                           UserRoleTypeEnum.Partner));

            if (result.Succeeded)
            {
                await this.Mediator.Send(
                    new CreatePartnerRequest(user.Id, model.FirstName, model.SecondName, model.Status));

                return this.Created($"/api/v2/partners/{user.Id}", new { partnerId = user.Id });
            }

            return this.BadRequest(new Tools.ApiResponse<string>(result.Errors.Select(x => x.Description)));
        }
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
    }
}