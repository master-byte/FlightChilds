using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.BusinessLogic.Requests.Common.Tariffs;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.Core.Constants;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Users
{
    [Authorize(Policy = Policies.Admin)]
    public class Create : EndpointBase
    {
        [HttpPost("admin/parents")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute(UserCreateModel createModel)
        {
            var (user, identityResult) = await this.Mediator.Send(
                                             new RegisterUserRequest(
                                                 createModel.PhoneNumber,
                                                 null,
                                                 createModel.PhoneNumber,
                                                 createModel.Email,
                                                 true,
                                                 createModel.ParentFullName,
                                                 UserRoleTypeEnum.Parent));

            if (identityResult.Succeeded)
            {
                var userIdentity = new UserIdentity(user.Id);

                var child = await this.Mediator.Send(
                                new CreateDefaultChildRequest(
                                    userIdentity,
                                    createModel.BirthDate,
                                    createModel.ChildFirstName,
                                    createModel.Gender,
                                    createModel.ChildSecondName,
                                    true));

                await this.Mediator.Send(new SetTariffRequest(userIdentity, TariffsConstants.FreeTariffId));
                await this.Mediator.Send(
                    new CreateDefaultPortfolioRequest(
                        child.ChildId,
                        createModel.HairColor,
                        createModel.HairLength,
                        createModel.Height,
                        createModel.EyeColors,
                        createModel.AppearanceType,
                        createModel.Experience,
                        createModel.About,
                        createModel.SocialNetworks));
            }
            else
            {
                return this.BadRequest(new ApiResponse<string>(identityResult.Errors.Select(x => x.Description)));
            }

            return this.Ok();
        }

        public class UserCreateModel
        {
            [Phone]
            [Required]
            [MaxLength(16)]
            public string PhoneNumber { get; set; }

            [MaxLength(250)]
            public string ParentFullName { get; set; }

            [EmailAddress]
            [MaxLength(200)]
            public string Email { get; set; }

            [Range(typeof(DateTime), "1/1/1900", "1/1/2100")]
            public DateTime? BirthDate { get; set; }

            [MaxLength(125)]
            public string ChildFirstName { get; set; }

            [MaxLength(125)]
            public string ChildSecondName { get; set; }

            [MaxLength(5000)]
            public string Experience { get; set; }

            [MaxLength(5000)]
            public string About { get; set; }

            [MaxLength(1000)]
            public string SocialNetworks { get; set; }

            [EnumDataType(typeof(Gender))]
            public Gender? Gender { get; set; }

            [EnumDataType(typeof(HairColor))]
            public HairColor? HairColor { get; set; }

            [EnumDataType(typeof(HairLength))]
            public HairLength? HairLength { get; set; }

            [EnumDataType(typeof(EyeColor))]
            public EyeColor? EyeColors { get; set; }

            [EnumDataType(typeof(AppearanceType))]
            public AppearanceType? AppearanceType { get; set; }

            [Range(40, 230)]
            public int? Height { get; set; }
        }
    }
}