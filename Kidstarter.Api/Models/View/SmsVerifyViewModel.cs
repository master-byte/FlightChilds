using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models.View
{
    public class SmsVerifyViewModel
    {
        [Required]
        [RegularExpression(@"^(?:\+7)\d{10}$")]
        public string PhoneNumber { get; set; }
    }
}
