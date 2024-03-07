using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models.Create
{
    public sealed class SupportQuestionCreateModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(2000)]
        public string Text { get; set; }
    }
}