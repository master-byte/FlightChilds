using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Mappings;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.Update;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.BusinessLogic.Requests.Organizer.Users;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.Dto;
using Kidstarter.Core.Models.EF.Portfolios;
using Kidstarter.Core.Models.Filters;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private static readonly DictionaryListDto DictionaryList = new();

        private static readonly EventTypeListDto EventTypeList = new();

        private readonly IMediator mediator;

        private readonly IMapper mapper;

        private readonly SplitUserMapperService splitUserMapperService;

        public UserController(IMediator mediator, IMapper mapper, SplitUserMapperService splitUserMapperService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.splitUserMapperService = splitUserMapperService;
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<UserViewModel>> GetMyProfile()
        {
            var (userViewModel, _) = await this.splitUserMapperService.Map<UserViewModel>(new GetUserProfileRequest(this.GetCurrentUser()));
            return this.ApiOk(userViewModel);
        }

        [Authorize(Policy = Policies.Organizer)]
        [HttpGet("{userId}")]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<UserViewModel>> GetUserInfo(Guid userId)
        {
            var (userViewModel, _) = await this.splitUserMapperService.Map<UserViewModel>(new GetUserProfileRequest(userId));

            return this.ApiOk(userViewModel);
        }

        [HttpPatch("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<UserViewModel>> UpdateMyProfile([FromBody] UserUpdateModel updateModel)
        {
            await this.mediator.Send(new UpdateUserProfileRequest(updateModel.GetUpdateAction()));

            var defaultChild = await this.mediator.Send(new GetDefaultChildRequest());
            if (defaultChild != null)
            {
                await this.mediator.Send(
                    new UpdateChildRequest(
                        defaultChild.ChildId,
                        c =>
                            {
                                c.FirstName = updateModel.FirstName ?? c.FirstName;
                                c.SecondName = updateModel.SecondName ?? c.SecondName;
                                c.Gender = updateModel.Gender ?? c.Gender;
                                c.BirthDate = updateModel.BirthDate ?? c.BirthDate;
                            }));

                var portfolios = await this.mediator.Send(new GetPortfoliosRequest(defaultChild.ChildId));
                var filmingPortfolio = portfolios.FirstOrDefault(x => x.PortfolioType == "Casting");
                if (filmingPortfolio != null)
                {
                    await this.mediator.Send(
                        new UpdatePortfolioRequest(
                            filmingPortfolio.PortfolioId,
                            p =>
                                {
                                    if (p is FilmingPortfolio fp)
                                    {
                                        fp.HairColor = updateModel.HairColor ?? fp.HairColor;
                                        fp.HairLength = updateModel.HairLength ?? fp.HairLength;
                                        fp.Height = (int?)updateModel.Height ?? fp.Height;
                                        fp.EyeColors = updateModel.EyeColors ?? fp.EyeColors;
                                        fp.AppearanceType = updateModel.AppearanceType ?? fp.AppearanceType;
                                        fp.Experience = updateModel.Experience ?? fp.Experience;
                                        fp.About = updateModel.About ?? fp.About;
                                        fp.SocialNetworks = updateModel.SocialNetworks ?? fp.SocialNetworks;
                                    }
                                }));
                }
            }

            return await this.GetMyProfile();
        }

        [HttpPatch]
        [Route("acceptLicenseAgreement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<bool>> AcceptLicenseAgreement()
        {
            await this.mediator.Send(new UpdateUserProfileRequest(x => x.AcceptedUserAgreement = true));

            return this.ApiOk(true);
        }

        [HttpGet("dictionaryList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ApiResponse<DictionaryListDto> GetDictionaryList()
        {
            return this.ApiOk(DictionaryList);
        }

        [HttpGet("eventTagTypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ApiResponse<EventTypeListDto> GetEventTagTypes()
        {
            return this.ApiOk(EventTypeList);
        }

        [HttpGet("listByFiltered")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<ListDto<UserViewModel>>> Search(
            [FromQuery] UserProfileFilter filter,
            [FromQuery] Pager pager)
        {
            var result = await this.mediator.Send(new FindUsersRequest(filter, pager.ToPagedRequest()));
            var viewModels = result.ChangeType(
                items => items.Select(x => this.mapper.MapSplitEntitiesIntoUser(x)).ToArray());

            return this.ApiOk(new ListDto<UserViewModel>(viewModels));
        }

        [HttpDelete]
        [Route("upload/{uploadId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<string[]>> DeleteUpload(Guid uploadId)
        {
            string[] results;
            var defaultChild = await this.mediator.Send(new GetDefaultChildRequest());
            if (defaultChild != null)
            {
                results = await this.mediator.Send(new DeleteChildMediaFileRequest(defaultChild.ChildId, new[] { uploadId }));
            }
            else
            {
                results = await this.mediator.Send(new DeleteUserMediaFileRequest(new[] { uploadId }));
            }

            return this.ApiOk(results);
        }
    }
}
