using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace Kidstarter.Api.Models
{
    public class AppleReceiptModel
    {
        [JsonProperty("receipt-data")]
        [Required(AllowEmptyStrings = false)]
        public string ReceiptData { get; set; }
    }
}