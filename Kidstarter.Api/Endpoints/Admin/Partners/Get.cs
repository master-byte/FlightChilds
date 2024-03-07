using System;
using System.Threading;
using System.Threading.Tasks;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Admin.Partners.Extensions;
using Kidstarter.Api.Endpoints.Admin.Partners.Models;
using Kidstarter.BusinessLogic.Requests.Admin.Partners;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Partners
{
    [Authorize(Policy = Policies.Admin)]
    public class Get : EndpointBase
    {
        [HttpGet("admin/partners/{partnerId:guid}")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartnerDetailsViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HandleAsync([FromRoute] Guid partnerId, CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(new GetPartnerDetailsRequest(partnerId.ToString()), cancellationToken);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result.ToDetailsViewModel());
        }
    }
}
