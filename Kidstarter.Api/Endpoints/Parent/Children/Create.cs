using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Children.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class Create : EndpointBase
    {
        [HttpPost("parent/children")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<ChildViewModel>> HandleAsync(
            [FromBody] CreateChildModel model,
            CancellationToken cancellationToken = default)
        {
            var request = new CreateChildRequest(
                this.Mapper.Map<Child>(model, opt => opt.Items["ParentId"] = this.GetCurrentUser().Id));

            var result = await this.Mediator.Dispatch(
                             request,
                             this.Mapper.Map<ChildViewModel>,
                             cancellationToken);

            return this.ApiCreated($"/api/v2/children/{result.ChildId}", result);
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
    }
}
