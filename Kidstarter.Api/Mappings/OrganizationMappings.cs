using System.Linq;

using AutoMapper;

using Kidstarter.Api.Endpoints.Common.Organizations;
using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;

namespace Kidstarter.Api.Mappings
{
    internal sealed class OrganizationMappings : Profile
    {
        public OrganizationMappings()
        {
            this.ToEntities();
            this.FromEntities();
        }

        private void ToEntities()
        {
        }

        private void FromEntities()
        {
            this.CreateMap<OrganizationContacts, OrganizationContactsViewModel>();
            this.CreateMap<OrganizationBusinessHours, BusinessHoursViewModel>();
            this.CreateMap<EventDirection, DirectionViewModel>();
            this.CreateMap<EventDirection, DirectionViewModelV1>();
            this.CreateMap<Partner, PartnerViewModel>();

            this.CreateMap<OrganizationViewModel, OrganizationModel>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.IsFavourite, opt => opt.MapFrom(src => src.IsFavourite))
                .ReverseMap();

            this.CreateMap<OrganizationViewModelV1, OrganizationModel>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.IsFavourite, opt => opt.MapFrom(src => src.IsFavourite))
                .ReverseMap();

            this.CreateMap<Organization, OrganizationViewModel>()
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.GeoLocation != null ? src.GeoLocation.Y : (double?)null))
                .ForMember(dest => dest.Lon, opt => opt.MapFrom(src => src.GeoLocation != null ? src.GeoLocation.X : (double?)null))
                .ForMember(
                    dest => dest.Videos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)))
                .ForMember(dest => dest.Directions, opt => opt.MapFrom(src => src.Directions.Select(x => x.EventDirection)));

            this.CreateMap<Organization, OrganizationViewModelV1>()
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.GeoLocation != null ? src.GeoLocation.Y : (double?)null))
                .ForMember(dest => dest.Lon, opt => opt.MapFrom(src => src.GeoLocation != null ? src.GeoLocation.X : (double?)null))
                .ForMember(
                    dest => dest.Videos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)))
                .ForMember(dest => dest.Directions, opt => opt.MapFrom(src => src.Directions.Select(x => x.EventDirection)));
        }
    }
}
