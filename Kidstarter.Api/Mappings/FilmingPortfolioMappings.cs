using AutoMapper;

using Kidstarter.Api.Endpoints.Parent.Portfolio.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.EF.Portfolios;

namespace Kidstarter.Api.Mappings
{
    public class FilmingPortfolioMappings : Profile
    {
        public FilmingPortfolioMappings()
        {
            this.ToEntities();
            this.FromEntities();
        }

        private void FromEntities()
        {
            this.CreateMap<PortfolioBase, PortfolioViewModelBase>()
                .IncludeAllDerived();

            this.CreateMap<FilmingPortfolio, FilmingPortfolioViewModel>();
        }

        private void ToEntities()
        {
            this.CreateMap<PortfolioCreateModelBase, PortfolioBase>()
                .IncludeAllDerived();

            this.CreateMap<FilmingPortfolioUpdateModel, PortfolioBase>()
                .IncludeAllDerived();

            this.CreateMap<FilmingPortfolioCreateModel, FilmingPortfolio>();
            this.CreateMap<FilmingPortfolioUpdateModel, FilmingPortfolio>()
                .ForMember(dest => dest.ChildId, opt => opt.Ignore())
                .ForAllOtherMembers(opts => opts.Condition((_, _, srcMember) => srcMember is not null));
        }
    }
}