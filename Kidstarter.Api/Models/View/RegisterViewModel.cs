using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models.View
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^(?:\+7)\d{10}$")]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
    }
}