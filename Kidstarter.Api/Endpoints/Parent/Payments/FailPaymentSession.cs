using System;

using Kidstarter.Api.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kidstarter.Api.Endpoints.Parent.Payments
{
    [AllowAnonymous]
    [Route("api/payments")]
    public class FailPaymentSession : V1EndpointBase
    {
        private readonly Uri applicationHost;

        public FailPaymentSession(IOptions<ApplicationSettings> applicationSettings)
        {
            this.applicationHost = new Uri(applicationSettings.Value.Host);
        }

        [HttpGet]
        [Route("fail/{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
        public IActionResult Fail(Guid orderId)
        {
            return this.RedirectPermanent(new Uri(this.applicationHost, "/fail.html").AbsoluteUri);
        }
    }
}
