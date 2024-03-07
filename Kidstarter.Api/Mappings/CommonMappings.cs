using AutoMapper;

using Kidstarter.Api.Models.View;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Http;

namespace Kidstarter.Api.Mappings
{
    internal sealed class CommonMappings : Profile
    {
        public CommonMappings()
        {
            this.ToEntities();
            this.FromEntities();
            this.FromModels();

            this.CreateMap<(IFormFile File, MediaClass MediaClass), MediaUploadRequest>()
                .ForMember(dest => dest.FileStream, opt => opt.MapFrom(src => src.File.OpenReadStream()))
                .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.File.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.ContentType))
                .ForMember(dest => dest.MediaClass, opt => opt.MapFrom(src => src.MediaClass));

            this.CreateMap<IFormFile, MediaUploadRequest>()
                .ForMember(dest => dest.FileStream, opt => opt.MapFrom(src => src.OpenReadStream()))
                .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType));
        }

        private void FromModels()
        {
        }

        private void ToEntities()
        {
        }

        private void FromEntities()
        {
            this.CreateMap<SupportQuestion, SupportQuestionViewModel>();
            this.CreateMap<Tariff, TariffViewModel>()
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Price));

            this.CreateMap<Upload, UploadViewModel>();
            this.CreateMap<UploadThumbnail, UploadThumbnailViewModel>();
        }
    }
}