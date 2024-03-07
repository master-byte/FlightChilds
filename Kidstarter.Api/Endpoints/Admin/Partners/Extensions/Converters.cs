using Kidstarter.Api.Endpoints.Admin.Partners.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.BusinessLogic.Models;

using Nelibur.ObjectMapper;

namespace Kidstarter.Api.Endpoints.Admin.Partners.Extensions
{
    internal static class Converters
    {
        public static PartnerDetailsViewModel ToDetailsViewModel(this PartnerDetailsModel model)
        {
            return new()
            {
                Partner = TinyMapper.Map<PartnerViewModel>(model.Partner),
                Tariff = new()
                {
                    Tariff = TinyMapper.Map<TariffViewModel>(model.UserTariff.Tariff),
                    CreatedAt = model.UserTariff.CreatedAt,
                    ExpiresAt = model.UserTariff.ExpiresAt
                }
            };
        }
    }
}