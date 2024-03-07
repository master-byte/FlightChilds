using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Kidstarter.Api.Controllers.Admin.Models;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Mappings.Resolvers;
using Kidstarter.Api.Models.Base;
using Kidstarter.Api.Models.Create;
using Kidstarter.Api.Models.View;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Mappings
{
    internal class UserMappings : Profile
    {
        public UserMappings()
        {
            this.CreateMap<List<UserRole>, UserRoleTypeEnum?>().ConvertUsing<RolesResolver>();

            this.ToEntities();
            this.FromEntities();
        }

        private void FromEntities()
        {
            this.CreateMap<User, ModelWithMediaBase>()
                .IncludeAllDerived()
                .ForMember(dest => dest.Videos, opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)));

            this.CreateMap<User, TokenUserViewModelV1>()
                .ForMember(dest => dest.ProducerCenterId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Uploads.Select(x => x.Upload)))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.UserRoles));

            this.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.ProducerCenterId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Uploads.Select(x => x.Upload)))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.UserRoles));

            this.CreateMap<User, ParentViewModel>();

            this.CreateMap<User, OrganizerViewModel>();

            this.CreateMap<UserTariff, UserTariffViewModel>();
        }

        private void ToEntities()
        {
            this.CreateMap<UserCreateModel, User>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.GetAge()));
        }
    }
}