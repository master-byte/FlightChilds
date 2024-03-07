using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Parent.Children.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Children
{
    [VersionedEndpoint]
    [Authorize(Policy = Policies.Parent)]
    public class Update : EndpointBase
    {
        [HttpPatch("parent/children/{childId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<ChildViewModel>> HandleAsync(
            [FromRoute] Guid childId,
            [FromBody] ChildUpdateModel request,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Dispatch(
                             new UpdateChildRequest(childId, c => this.Mapper.Map(request, c)),
                             this.Mapper.Map<ChildViewModel>,
                             cancellationToken);

            return this.ApiOk(result);
        }
    }

    public class ChildUpdateModel
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