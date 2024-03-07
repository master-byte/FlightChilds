using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Tariffs;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Parents
{
    [Authorize(Policy = Policies.Admin)]
    [Route("admin/parents")]
    public sealed class ChangeTariff : EndpointBase
    {
        [HttpPost("{parentId:guid}/tariff")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Execute([FromRoute] Guid parentId, [FromBody] TariffChangeModel tariffChangeModel)
        {
            var parent = new UserIdentity(parentId, UserRoleTypeEnum.Parent);

            await this.Mediator.Send(new SetTariffRequest(parent, tariffChangeModel.NewTariffId, tariffChangeModel.ExpiresAt));

            return this.NoContent();
        }
    }

    public class TariffChangeModel
    {
        [Range(1, 100)]
        public int NewTariffId { get; set; }

        public DateTimeOffset? ExpiresAt { get; set; }
    }
}