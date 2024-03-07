using System;

namespace Kidstarter.Api.Models.View
{
    public class UserTariffViewModel
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ExpiresAt { get; set; }

        public TariffViewModel Tariff { get; set; }
    }
}