using System;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Configuration;
using Kidstarter.Api.Models;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Parent.Payments;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kidstarter.Api.Endpoints.Parent.Payments
{
    [Authorize(Policy = Policies.Parent)]
    [Route("api/payments")]
    public class StartPaymentSession : V1EndpointBase
    {
        private readonly Uri applicationHost;

        public StartPaymentSession(IOptions<ApplicationSettings> applicationSettings)
        {
            this.applicationHost = new Uri(applicationSettings.Value.Host);
        }

        [HttpPost]
        [Route("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public Task<StartSessionResponse> StartSession(PaymentRequest request)
        {
            return this.Mediator.Send(
                             new StartPaymentSessionRequest(
                                 (int)request.TariffId,
                                 request.Email,
                                 Guid.NewGuid(),
                                 this.applicationHost));
        }
    }
}
