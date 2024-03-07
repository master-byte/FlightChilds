namespace Kidstarter.Api.Models
{
    public class Pager
    {
        public uint Page { get; set; }

        public uint PageSize { get; set; } = int.MaxValue;
    }
}