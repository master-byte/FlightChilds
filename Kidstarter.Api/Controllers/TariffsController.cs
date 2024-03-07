using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Common.Tariffs;
using Kidstarter.BusinessLogic.Requests.Parent.Tariffs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Policy = Policies.Parent)]
    public sealed class TariffsController : ControllerBase
    {
        private readonly IMediator mediator;

        private readonly IMapper mapper;

        public TariffsController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<TariffViewModel[]>> Get()
        {
            var tariffs = await this.mediator.Send(new GetAllTariffsRequest());

            return this.ApiOk(this.mapper.Map<TariffViewModel[]>(tariffs));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{tariffId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<TariffViewModel>> GetById([Range(1, uint.MaxValue)] uint tariffId)
        {
            var tariff = await this.mediator.Send(new GetTariffRequest(tariffId));
            if (tariff == null)
            {
                return this.ApiNotFound<TariffViewModel>();
            }

            return this.ApiOk(this.mapper.Map<TariffViewModel>(tariff));
        }

        [HttpGet]
        [Route("my")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<UserTariffViewModel>> GetMyTariff()
        {
            var tariff = await this.mediator.Send(new GetUserTariffRequest());

            return this.ApiOk(this.mapper.Map<UserTariffViewModel>(tariff));
        }

        [HttpGet]
        [Route("my/limits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<CurrentUserLimitModel[]>> GetLimits()
        {
            var limits = await this.mediator.Send(new GetTariffLimitsRequest());

            return this.ApiOk(limits);
        }

        [HttpGet]
        [Route("isTrialAvailable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<bool>> IsTrialAvailable()
        {
            var available = await this.mediator.Send(new IsTrialAvailableRequest());

            return new ApiResponse<bool>(available);
        }

        [HttpPost]
        [Route("startTrial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<bool>> StartTrial()
        {
            await this.mediator.Send(new StartTrialRequest());

            return new ApiResponse<bool>(true);
        }
    }
}