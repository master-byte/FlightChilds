using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Organizations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Organizations
{
    [Authorize(Policy = Policies.Admin)]
    public class BulkCreate : EndpointBase
    {
        [HttpPost("admin/organizations/bulk")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromBody] OrganizationCreateModel[] createModels)
        {
            foreach (var createModel in createModels)
            {
                await this.Mediator.Send(new CreateOrganizationRequest(createModel.ToOrganization()));
            }

            return this.Ok();
        }
    }
}