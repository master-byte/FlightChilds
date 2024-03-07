using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models
{
    public sealed class PaymentRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public uint TariffId { get; set; }
    }
}