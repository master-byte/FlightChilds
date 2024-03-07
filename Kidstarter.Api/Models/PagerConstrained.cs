using System.ComponentModel.DataAnnotations;

namespace Kidstarter.Api.Models
{
    public sealed class PagerConstrained
    {
        private const int DefaultPageSize = 20;

        [Range(0, 10000)]
        public int Page { get; set; }

        [Range(10, 50)]
        public int PageSize { get; set; } = DefaultPageSize;
    }
}