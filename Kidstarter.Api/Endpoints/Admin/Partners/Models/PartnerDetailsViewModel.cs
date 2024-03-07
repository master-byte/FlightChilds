using Kidstarter.Api.Models.View;

namespace Kidstarter.Api.Endpoints.Admin.Partners.Models
{
    public class PartnerDetailsViewModel
    {
        public PartnerViewModel Partner { get; set; }

        public UserTariffViewModel Tariff { get; set; }
    }
}