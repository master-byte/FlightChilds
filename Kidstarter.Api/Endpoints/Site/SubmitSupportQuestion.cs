using System.Threading.Tasks;

using Kidstarter.Api.Models.Create;
using Kidstarter.Api.Tools;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Infrastructure.EF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Site
{
    [AllowAnonymous]
    public sealed class SubmitSupportQuestion : EndpointBase
    {
        private readonly ApplicationDbContext context;

        public SubmitSupportQuestion(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        [Route("site/supportQuestions")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(ApiResponse<string>))]
        public async Task<IActionResult> Post([FromBody] SupportQuestionCreateModel createModel)
        {
            this.context.Set<SupportQuestion>()
                .Add(new()
                {
                    UserId = null,
                    Status = SupportQuestionStatus.Active,
                    Text = createModel.Text,
                });

            await this.context.SaveChangesAsync();

            return this.StatusCode(StatusCodes.Status201Created);
        }
    }
}