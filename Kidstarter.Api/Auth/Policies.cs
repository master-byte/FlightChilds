using Microsoft.AspNetCore.Authorization;

namespace Kidstarter.Api.Auth
{
    public sealed class Policies
    {
        public const string Admin = "Админ";

        public const string Parent = "Родитель";

        public const string Producer = "Продюсер";

        public const string Organizer = "Организатор";

        public const string Partner = "Партнер";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Admin)
                .Build();
        }

        public static AuthorizationPolicy ParentPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Parent)
                .Build();
        }

        public static AuthorizationPolicy ProducerPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Producer)
                .Build();
        }

        public static AuthorizationPolicy OrganizerPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Producer, Organizer)
                .Build();
        }

        public static AuthorizationPolicy PartnerPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Partner)
                .Build();
        }
    }
}