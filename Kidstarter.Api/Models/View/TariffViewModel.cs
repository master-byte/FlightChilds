namespace Kidstarter.Api.Models.View
{
    public sealed class TariffViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price => this.OriginalPrice / 100.0M;

        public decimal OriginalPrice { get; set; }

        public decimal AppStorePrice { get; set; }

        public string Currency { get; set; }

        public string IconId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int NumberOfMonths { get; set; }

        public int NumberOfDays { get; set; }
    }
}