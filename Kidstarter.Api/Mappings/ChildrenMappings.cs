using System.Linq;

using AutoMapper;

using Kidstarter.Api.Endpoints.Parent.Children;
using Kidstarter.Api.Models.Base;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;

namespace Kidstarter.Api.Mappings
{
    internal class ChildrenMappings : Profile
    {
        public ChildrenMappings()
        {
            this.ToEntities();
            this.FromEntities();
        }

        private void FromEntities()
        {
            this.CreateMap<Child, ModelWithMediaBase>()
                .IncludeAllDerived()
                .ForMember(
                    dest => dest.Videos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)));
        }

        private void ToEntities()
        {
            this.CreateMap<CreateChildModel, Child>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom((_, _, _, context) => context.Items["ParentId"]));

            this.CreateMap<ChildUpdateModel, Child>()
                .ForAllOtherMembers(opts => opts.Condition((_, _, srcMember) => srcMember is not null));
        }
    }
}