using System.Threading.Tasks;

using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Endpoints.Common.Workflow
{
    [Authorize]
    public class UserLogOut : EndpointBase
    {
        [HttpPost]
        [Route("logout")]
        [MapToApiVersion("2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse> Execute(LogOutModel model)
        {
            if (!string.IsNullOrEmpty(model.DeviceToken))
            {
                await this.Mediator.Send(new DeleteUserDeviceTokenRequest(model.DeviceToken));
            }

            return this.ApiOk(true);
        }

        public class LogOutModel
        {
            public string DeviceToken { get; set; }
        }
    }
}