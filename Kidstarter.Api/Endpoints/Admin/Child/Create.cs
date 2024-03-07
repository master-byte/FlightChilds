using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [Authorize(Policy = Policies.Admin)]
    public class Create : EndpointBase
    {
        [HttpPost("admin/children")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateChildModel createModel)
        {
            await this.Mediator.ExecuteAs(
                new CreateChildRequest(
                    new()
                    {
                        FirstName = createModel.FirstName,
                        BirthDate = createModel.BirthDate,
                        Gender = createModel.Gender,
                        SecondName = createModel.SecondName,
                        ParentId = createModel.ParentId,
                        Uploads = createModel.Uploads.Select(
                        x => new ChildUpload
                        {
                            UploadId = x
                        }).ToList()
                    }),
                new(createModel.ParentId, UserRoleTypeEnum.Parent));

            return this.Ok();
        }
    }

    public class CreateChildModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string SecondName { get; set; }

        [Range(typeof(DateTime), "1/1/1995", "1/1/2100")]
        public DateTime? BirthDate { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(60)]
        public string ParentId { get; set; }

        public string[] Uploads { get; set; }
    }
}