using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Admin.SupportQuestions;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Admin.SupportQuestions
{
    [Authorize(Policy = Policies.Admin)]
    public class Find : EndpointBase
    {
        [HttpGet("admin/supportQuestions/find")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<ListDto<SupportQuestionViewModel>>> Execute(
            [FromQuery] PagerConstrained pager,
            CancellationToken cancellationToken = default)
        {
            var questions = await this.Mediator.Send(
                new FindSupportQuestionsRequest(pager.ToPagedRequest()),
                cancellationToken);

            var result = questions.ChangeType(
                x => x.Select(
                    n => new SupportQuestionViewModel
                    {
                        Id = n.Id,
                        Text = n.Text,
                        CreatedAt = n.CreatedAt,
                        Status = n.Status
                    }));

            return this.ApiOk(new ListDto<SupportQuestionViewModel>(result));
        }
    }
}
