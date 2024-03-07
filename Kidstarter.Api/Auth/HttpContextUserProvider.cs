using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

using Kidstarter.Core.Exceptions;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Workflow;

using Microsoft.AspNetCore.Http;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace Kidstarter.Api.Auth
{
    internal sealed class HttpContextUserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private UserIdentity user;

        private UserScope scope;

        public HttpContextUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        [DebuggerStepThrough]
        public UserIdentity Get()
        {
            if (this.scope != null)
            {
                return this.scope.User;
            }

            var principal = this.httpContextAccessor.HttpContext?.User;
            if (principal == null)
            {
                throw new NotAuthorizedException("Пользователь не авторизован");
            }

            this.user = GetUserIdentity(principal);
            this.scope = new UserScope(null, this.user, this.RestoreScope);

            return this.user;
        }

        public IDisposable CreateScope(UserIdentity scopeUser)
        {
            Guard.ArgumentNotNull(scopeUser, nameof(scopeUser));

            return this.scope = new UserScope(this.scope, scopeUser, this.RestoreScope);
        }

        private static UserIdentity GetUserIdentity(ClaimsPrincipal principal)
        {
            var nameClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = principal.FindAll(ClaimTypes.Role).Select(x => EnumExtensions.ToEnum<UserRoleTypeEnum>(x.Value));

            return string.IsNullOrEmpty(nameClaim)
                       ? null
                       : new UserIdentity(Guid.Parse(nameClaim), roles.ToArray());
        }

        private void RestoreScope()
        {
            this.scope = this.scope.ParentScope;
        }

        private class UserScope : IDisposable
        {
            private readonly Action restoreScope;

            public UserScope(UserScope parentScope, UserIdentity user, Action restoreScope)
            {
                this.ParentScope = parentScope;
                this.User = user;
                this.restoreScope = restoreScope;
            }

            public UserScope ParentScope { get; }

            public UserIdentity User { get; }

            public void Dispose() => this.restoreScope();
        }
    }
}