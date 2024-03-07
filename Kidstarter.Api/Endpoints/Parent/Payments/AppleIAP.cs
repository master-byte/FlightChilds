using System;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.Payments;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Parent.Payments
{
    [Authorize(Policy = Policies.Parent)]
    [Route("api/payments")]
    public class AppleIAP : V1EndpointBase
    {
        [HttpPost]
        [Route("apple/completeIAP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteAppleIAP(AppleReceiptModel appleReceipt)
        {
            if (!this.IsAppleDevice())
            {
                return this.BadRequest(this.ApiError("Method only allowed for iOS devices."));
            }

            var result = await this.Mediator.Send(
                             new CompleteAppleIAPRequest(appleReceipt.ReceiptData));

            return this.Ok(result);
        }

        private bool IsAppleDevice() => this.Request.Headers["platform"].ToString().Equals("iOS", StringComparison.OrdinalIgnoreCase);
    }
}
