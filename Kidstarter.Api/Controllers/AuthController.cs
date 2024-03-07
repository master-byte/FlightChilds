using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Configuration;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Mappings;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Common;
using Kidstarter.BusinessLogic.Requests.Common.Tariffs;
using Kidstarter.BusinessLogic.Requests.Common.Users;
using Kidstarter.BusinessLogic.Services;
using Kidstarter.Core.Constants;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Dto;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Workflow;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Kidstarter.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;

        private readonly JwtSecurityTokenHandler tokenHandler = new();

        private readonly ISmsCodesService smsService;

        private readonly ApplicationSettings settings;

        private readonly AuthSettings authSettings;

        private readonly IMapper mapper;

        private readonly IMediator mediator;

        private readonly SplitUserMapperService splitUserMapperService;

        private readonly IUserProvider userProvider;

        public AuthController(
            SignInManager<User> signInManager,
            ISmsCodesService smsService,
            IOptions<ApplicationSettings> settings,
            IOptions<AuthSettings> authSettings,
            IMapper mapper,
            IMediator mediator,
            SplitUserMapperService splitUserMapperService,
            IUserProvider userProvider)
        {
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.mediator = mediator;
            this.splitUserMapperService = splitUserMapperService;
            this.userProvider = userProvider;
            this.smsService = smsService;
            this.settings = settings.Value;
            this.authSettings = authSettings.Value;
        }

        [HttpGet("self")]
        [Authorize]
        public async Task<ApiResponse<UserViewModel>> GetMyProfile()
        {
            var viewModel = await this.splitUserMapperService.Map<UserViewModel>(new GetUserProfileRequest(this.GetCurrentUser()));
            return this.ApiOk(viewModel.ViewModel);
        }

        [HttpPost("token")]
        public async Task<ApiResponse<TokenViewModel>> Token([FromBody] RegisterViewModel vm)
        {
            var (_, user) = await this.splitUserMapperService.Map<TokenUserViewModelV1>(new GetUserProfileRequest(vm.PhoneNumber));
            if (user == null)
            {
                return this.ApiError<TokenViewModel>("Имя пользователя или пароль неверны.");
            }

            var result = await this.signInManager.PasswordSignInAsync(vm.PhoneNumber, vm.Password, true, true);

            return result.Succeeded
                       ? this.ApiOk(await this.SignIn(user))
                       : this.ApiError<TokenViewModel>("Имя пользователя или пароль неверны.");
        }

        [AllowAnonymous]
        [HttpPost("smsPhoneVerify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<TokenViewModel>> SmsPhoneVerify([FromBody] SmsVerifyViewModel model)
        {
            var smsCode = this.smsService.GetSmsCode(model.PhoneNumber);
            bool success;

            if (this.settings.PhoneNumbersWhitelist.Contains(model.PhoneNumber))
            {
                smsCode.SmsCode = model.PhoneNumber.Substring(model.PhoneNumber.Length - 4, 4);
                success = true;
            }
            else
            {
                var platform = DeviceType.Unknown;
                if (this.Request.Headers.TryGetValue("platform", out var platformString))
                {
                    platform = Enum.Parse<DeviceType>(platformString, true);
                }

                success = await this.mediator.Send(new SendSmsRequest(new(smsCode.PhoneNumber, smsCode.SmsCode, platform)));
            }

            if (!success)
            {
                return this.ApiError<TokenViewModel>("Ошибка при отправке смс");
            }

            var claims = new[] { new Claim("registerModel", JsonConvert.SerializeObject(smsCode)) };

            return this.ApiOk(
                new TokenViewModel
                {
                    Token = this.tokenHandler.WriteToken(this.IssueToken(claims)),
                    Message = "Ok"
                });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse<TokenViewModel>> Register([FromBody] SmsViewModel model)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(model.Token);
            var claims = jwtToken.Claims.First(claim => claim.Type == "registerModel").Value;

            var (decodedSmsCode, decodedPhoneNumber) = JsonConvert.DeserializeObject<SmsCodeDto>(claims)!;
            if (decodedSmsCode != model.SmsCode || jwtToken.ValidTo < DateTime.UtcNow)
            {
                return this.ApiError<TokenViewModel>("Ошибка при проверке СМС-кода");
            }

            var (_, user) = await this.splitUserMapperService.Map<UserViewModel>(new GetUserProfileRequest(decodedPhoneNumber));
            if (user != null)
            {
                await this.UpdateUserDeviceInfo(model.DeviceToken, model.TimeZone, Guid.Parse(user.Id));
                return this.ApiOk(await this.SignIn(user));
            }

            var result = await this.CreateUser(decodedPhoneNumber, model.DeviceToken, model.TimeZone);

            return result.Result.Succeeded
                       ? this.ApiOk(await this.SignIn(result.User))
                       : this.ApiError<TokenViewModel>(result.Result.Errors.First().Description);
        }

        private async Task<UserRegistrationResult> CreateUser(string userName, string deviceToken, string timeZone)
        {
            var result = await this.mediator.Send(new RegisterUserRequest(userName, null, userName, null, true, null, UserRoleTypeEnum.Parent));
            if (result.Result.Succeeded)
            {
                var userIdentity = new UserIdentity(result.User.Id);
                var child = await this.mediator.Send(new CreateDefaultChildRequest(userIdentity));
                await this.mediator.Send(new CreateDefaultPortfolioRequest(child.ChildId));
                await this.mediator.Send(new SetTariffRequest(userIdentity, TariffsConstants.FreeTariffId));
                await this.UpdateUserDeviceInfo(deviceToken, timeZone, Guid.Parse(result.User.Id));
            }

            return result;
        }

        private async Task UpdateUserDeviceInfo(string deviceToken, string timeZone, Guid userId)
        {
            var deviceInfo = !string.IsNullOrEmpty(deviceToken) && Enum.TryParse<DeviceType>(this.Request.Headers["platform"], true, out var platform)
                                 ? new(deviceToken, platform, timeZone)
                                 : (DeviceInformation?)null;

            if (deviceInfo != null)
            {
                await this.mediator.Send(new UpdateUserDeviceTokenRequest(deviceInfo.Value, userId));
            }
        }

        private async Task<TokenViewModel> SignIn(User user)
        {
            var principal = await this.signInManager.CreateUserPrincipalAsync(user);
            var claims = principal.Claims.ToList();

            var (userViewModel, _) = await this.splitUserMapperService.Map<TokenUserViewModelV1>(new(Guid.Parse(user.Id)));
            if (!string.IsNullOrEmpty(userViewModel.ProducerCenterId))
            {
                using var scope = this.userProvider.CreateScope(new(Guid.Parse(user.Id), UserRoleTypeEnum.Parent));
                var producerCenter = await this.mediator.Send(new GetOrganizationRequest(Guid.Parse(userViewModel.ProducerCenterId)));

                userViewModel.ProducerCenter = this.mapper.Map<OrganizationViewModel>(producerCenter);
                claims.Add(new Claim("OrganizationId", userViewModel.ProducerCenterId));
            }

            return new()
            {
                Message = "Ok",
                Token = this.tokenHandler.WriteToken(this.IssueToken(claims)),
                User = userViewModel
            };
        }

        private JwtSecurityToken IssueToken(IEnumerable<Claim> claims)
        {
            return new(
                 this.authSettings.Issuer,
                 this.authSettings.Audience,
                 claims,
                 this.authSettings.NotBefore,
                 this.authSettings.Expiration,
                 this.authSettings.SigningCredentials);
        }
    }
}