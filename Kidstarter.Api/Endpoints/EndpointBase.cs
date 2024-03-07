using AutoMapper;

using Kidstarter.Api.Tools;

using Lamar;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints
{
    [VersionedEndpoint]
    [ApiController]
    public class EndpointBase : ControllerBase
    {
        [SetterProperty]
        public IMediator Mediator { get; set; }

        [SetterProperty]
        public IMapper Mapper { get; set; }
    }
}