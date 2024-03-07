using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Controllers.Admin.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Parents;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Parents
{
    [Authorize(Policy = Policies.Admin)]
    [Route("admin/parents")]
    public sealed class Find : EndpointBase
    {
        private const string DefaultOrderColumn = "UserValueName";

        [HttpGet("find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListDto<ParentViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromQuery] UserListFilter filter, [FromQuery] PagerConstrained pager)
        {
            var request = new FindUsersRequest(
                filter.UserValueName,
                filter.PhoneNumber,
                filter.Email,
                filter.OrderBy ?? DefaultOrderColumn,
                filter.IsActive,
                pager.ToPagedRequest());

            var result = await this.Mediator.Send(request);

            var viewModels = result.ChangeType(this.Mapper.Map<ParentViewModel[]>);

            return this.Ok(new ListDto<ParentViewModel>(viewModels));
        }
    }

    public sealed class UserListFilter
    {
        [MaxLength(200)]
        public string UserValueName { get; set; }

        [MaxLength(16)]
        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(30)]
        public string OrderBy { get; set; }

        public bool? IsActive { get; set; }
    }
}
