using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models.View
{
    public class SmsViewModel
    {
        [Required(ErrorMessage = "Поле \"Код Sms\" не может быть пустым")]
        [MaxLength(10)]
        public string SmsCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(2000)]
        public string Token { get; set; }

        [MaxLength(1000)]
        public string DeviceToken { get; set; }

        [MaxLength(100)]
        public string TimeZone { get; set; }
    }
}