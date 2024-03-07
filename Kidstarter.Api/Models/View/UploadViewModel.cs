using System;
using System.Collections.Generic;

using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Models.View
{
    public sealed class UploadViewModel
    {
        public string Id { get; set; }

        public string CloudUrl { get; set; }

        public string ContentType { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MediaClass MediaClass { get; set; }

        public List<UploadThumbnailViewModel> Thumbnails { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}