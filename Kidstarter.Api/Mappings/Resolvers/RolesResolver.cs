using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Mappings.Resolvers
{
    internal sealed class RolesResolver : ITypeConverter<List<UserRole>, UserRoleTypeEnum?>
    {
        public UserRoleTypeEnum? Convert(List<UserRole> source, UserRoleTypeEnum? destination, ResolutionContext context)
        {
            var role = source.FirstOrDefault();
            if (role == null)
            {
                return null;
            }

            return role.Role.Name switch
            {
                "Родитель" => UserRoleTypeEnum.Parent,
                "Продюсер" => UserRoleTypeEnum.Producer,
                "Админ" => UserRoleTypeEnum.Admin,
                _ => null
            };
        }
    }
}