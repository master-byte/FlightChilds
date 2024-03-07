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
    public class CompletePaymentSession : V1EndpointBase
    {
        private readonly Uri applicationHost;

        public CompletePaymentSession(IOptions<ApplicationSettings> applicationSettings)
        {
            this.applicationHost = new Uri(applicationSettings.Value.Host);
        }

        [HttpGet]
        [Route("complete/{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public IActionResult Complete(Guid _)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            return this.RedirectPermanent(new Uri(this.applicationHost, "/success.html").AbsoluteUri);
        }
    }
}
