using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

using Kidstarter.Core.Exceptions;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Tools
{
    public static class Extensions
    {
        [DebuggerStepThrough]
        public static UserIdentity GetUserGuidId(this ClaimsPrincipal principal)
        {
            return GetUserIdentity(principal);
        }

        [DebuggerStepThrough]
        public static UserIdentity GetCurrentUser(this ControllerBase controller)
        {
            var principal = controller.User;
            if (principal == null)
            {
                throw new NotAuthorizedException("Пользователь не авторизован");
            }

            return GetUserIdentity(principal);
        }

        [DebuggerStepThrough]
        public static Guid? GetUserOrganizationId(this ControllerBase controller)
        {
            var principal = controller.User;
            if (principal == null)
            {
                throw new NotAuthorizedException("Пользователь не авторизован");
            }

            var orgClaim = principal.FindFirstValue("OrganizationId");

            return string.IsNullOrEmpty(orgClaim)
                       ? null
                       : Guid.Parse(orgClaim);
        }

        private static UserIdentity GetUserIdentity(ClaimsPrincipal principal)
        {
            var nameClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = principal.FindAll(ClaimTypes.Role).Select(x => EnumExtensions.ToEnum<UserRoleTypeEnum>(x.Value));

            return string.IsNullOrEmpty(nameClaim)
                       ? null
                       : new UserIdentity(Guid.Parse(nameClaim), roles.ToArray());
        }
    }
}
