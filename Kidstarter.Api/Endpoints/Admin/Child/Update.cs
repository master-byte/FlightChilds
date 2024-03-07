using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Services;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kidstarter.Api.Endpoints.Admin.Child
{
    [Authorize(Policy = Policies.Admin)]
    public class Update : EndpointBase
    {
        private readonly IEntityService entityService;

        public Update(IEntityService entityService)
        {
            this.entityService = entityService;
        }

        [HttpPut("admin/children/{childId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateOrganization([FromRoute] Guid childId, [FromBody] UpdateModel updateModel)
        {
            var parentId = await this.entityService.NoTrackingSet<Core.Models.EF.Child>()
                               .Where(x => x.ChildId == childId)
                               .Select(x => x.ParentId)
                               .SingleOrDefaultAsync();
            if (parentId == null)
            {
                return this.NotFound(new ApiResponse<string>("Родитель с указанным ID не найден."));
            }

            await this.Mediator.ExecuteAs(
                new UpdateChildRequest(
                    childId,
                    c =>
                        {
                            c.BirthDate = updateModel.BirthDate;
                            c.FirstName = updateModel.FirstName;
                            c.SecondName = updateModel.SecondName;
                            c.Gender = updateModel.Gender;
                        }),
                new(parentId, UserRoleTypeEnum.Parent));

            return this.Ok();
        }
    }

    public sealed class UpdateModel
    {
        [Range(typeof(DateTime), "1/1/1995", "1/1/2100")]
        public DateTime? BirthDate { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string SecondName { get; set; }
    }
}