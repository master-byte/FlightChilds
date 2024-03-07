using System;
using System.Collections.Generic;
using System.Linq;
using Kidstarter.Api.Models.View;

namespace Kidstarter.Api.Models.Base
{
    public abstract class ModelWithMediaBase
    {
        public IEnumerable<UploadViewModel> Photos { get; set; }

        public IEnumerable<UploadViewModel> Videos { get; set; }

        public IEnumerable<UploadViewModel> Images =>
            (this.Photos ?? Array.Empty<UploadViewModel>()).Union(this.Videos ?? Array.Empty<UploadViewModel>());
    }
}