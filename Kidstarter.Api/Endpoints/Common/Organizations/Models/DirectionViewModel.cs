namespace Kidstarter.Api.Endpoints.Common.Organizations.Models
{
    public class DirectionViewModel
    {
        public string ParentId { get; set; }

        public string EventDirectionId { get; set; }

        public string BackgroundUrl { get; set; }

        public string ForegroundUrl { get; set; }

        public string SmallLogoUrl { get; set; }

        public string LargeLogoUrl { get; set; }

        public string XLargeLogoUrl { get; set; }

        public string Name { get; set; }

        public int Level => string.IsNullOrEmpty(this.ParentId) ? 1 : 2;
    }
}