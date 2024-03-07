using System.ComponentModel.DataAnnotations;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Endpoints.Parent.Portfolio.Models
{
    public class FilmingPortfolioUpdateModel
    {
        [MaxLength(5000)]
        public string About { get; set; }

        [EnumDataType(typeof(AppearanceType))]
        public AppearanceType? AppearanceType { get; set; }

        [MaxLength(5000)]
        public string Experience { get; set; }

        [EnumDataType(typeof(EyeColor))]
        public EyeColor? EyeColors { get; set; }

        [EnumDataType(typeof(HairColor))]
        public HairColor? HairColor { get; set; }

        [EnumDataType(typeof(HairLength))]
        public HairLength? HairLength { get; set; }

        [Range(40, 230)]
        public int? Height { get; set; }

        [MaxLength(1000)]
        public string SocialNetworks { get; set; }
    }
}