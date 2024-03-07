using System;
using System.Threading.Tasks;

using Kidstarter.Api.Configuration;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Payments;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kidstarter.Api.Endpoints.Parent.Payments
{
    [AllowAnonymous]
    [VersionedEndpoint]
    public class CompletePaymentSessionV2 : EndpointBase
    {
        private readonly ILogger<CompletePaymentSessionV2> logger;

        private readonly ApplicationSettings settings;

        public CompletePaymentSessionV2(ILogger<CompletePaymentSessionV2> logger, IOptions<ApplicationSettings> settings)
        {
            this.logger = logger;
            this.settings = settings.Value;
        }

        [HttpPost]
        [Route("payments/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Complete([FromQuery]Guid token, [FromForm(Name = "order_id")] Guid orderId)
        {
            try
            {
                if (this.settings.ApiToken == token)
                {
                    await this.Mediator.Send(new CompletePaymentSessionRequest(orderId, OrderStatus.Completed));
                }
                else
                {
                    this.logger.LogWarning("Неверный API токен");
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Произошла ошибка");
            }

            return this.Ok();
        }
    }
}