using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Kidstarter.Api.Auth;
using Kidstarter.BusinessLogic.Requests.Admin.EventDirections;
using Kidstarter.Core.Models.EF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Nelibur.ObjectMapper;

namespace Kidstarter.Api.Endpoints.Admin.EventDirections
{
    [Authorize(Policy = Policies.Admin)]
    [Route("admin/eventDirections")]
    public class Create : EndpointBase
    {
        [HttpPost]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Execute(EventDirectionCreateModel[] createModel)
        {
            await this.Mediator.Send(new CreateEventDirectionRequest(createModel.Select(TinyMapper.Map<EventDirection>)));

            return this.NoContent();
        }
    }

    public class EventDirectionCreateModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(60)]
        public string EventDirectionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(60)]
        public string Name { get; set; }

        [MaxLength(60)]
        public string ParentId { get; set; }

        [Url]
        [MaxLength(1000)]
        public string BackgroundUrl { get; set; }

        [Url]
        [MaxLength(1000)]
        public string ForegroundUrl { get; set; }

        [Url]
        [MaxLength(1000)]
        public string SmallLogoUrl { get; set; }

        [Url]
        [MaxLength(1000)]
        public string LargeLogoUrl { get; set; }

        [Url]
        [MaxLength(1000)]
        public string XLargeLogoUrl { get; set; }
    }
}
