using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio.Models
{
    public class FilmingPortfolioViewModel : PortfolioViewModelBase
    {
        public string About { get; set; }

        public AppearanceType? AppearanceType { get; set; }

        public string Experience { get; set; }

        public EyeColor? EyeColors { get; set; }

        public HairColor? HairColor { get; set; }

        public HairLength? HairLength { get; set; }

        public int? Height { get; set; }

        public string SocialNetworks { get; set; }
    }
}