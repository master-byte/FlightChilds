using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Models.Create;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Parent.SupportQuestions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Родитель,Продюсер")]
    [Produces("application/json")]
    public sealed class SupportQuestionsController : ControllerBase
    {
        private readonly IMediator mediator;

        private readonly IMapper mapper;

        public SupportQuestionsController(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<SupportQuestionViewModel>> Post([FromBody] SupportQuestionCreateModel createModel)
        {
            var question = await this.mediator.Send(
                               new CreateSupportQuestionRequest(
                                   createModel.Text));

            return this.ApiCreated(this.mapper.Map<SupportQuestionViewModel>(question));
        }
    }
}
