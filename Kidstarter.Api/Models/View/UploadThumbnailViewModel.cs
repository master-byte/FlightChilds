namespace Kidstarter.Api.Models.View
{
    public sealed class UploadThumbnailViewModel
    {
        public string CloudUrl { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ContentType { get; set; }
    }
}