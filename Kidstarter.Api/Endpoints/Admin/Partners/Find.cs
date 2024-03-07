using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Admin.Partners.Extensions;
using Kidstarter.Api.Endpoints.Admin.Partners.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.Partners;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.Partners
{
    [Authorize(Policy = Policies.Admin)]
    public class Find : EndpointBase
    {
        [HttpGet("admin/partners/find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListDto<PartnerDetailsViewModel>))]
        public async Task<ApiResponse<ListDto<PartnerDetailsViewModel>>> HandleAsync(
            [FromQuery] PagerConstrained pager,
            CancellationToken cancellationToken = default)
        {
            var result = await this.Mediator.Send(
                       new FindPartnersRequest(pager.ToPagedRequest()),
                       cancellationToken);

            var viewModels = result.ChangeType(x => x.Select(n => n.ToDetailsViewModel()));

            return this.ApiOk(new ListDto<PartnerDetailsViewModel>(viewModels));
        }
    }
}
