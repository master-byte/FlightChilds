using AutoMapper;

using Lamar;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints
{
    [ApiController]
    public class V1EndpointBase : ControllerBase
    {
        [SetterProperty]
        public IMediator Mediator { get; set; }

        [SetterProperty]
        public IMapper Mapper { get; set; }
    }
}