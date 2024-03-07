using System;
using System.Linq;

using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Http;

namespace Kidstarter.Api.Models.Forms
{
    public sealed class FileUploadForm
    {
        public FileUploadForm()
        {
            this.MediaClass = Array.Empty<MediaClass>();
        }

        public IFormFile[] Media { get; set; }

        public MediaClass[] MediaClass { get; set; }

        public (IFormFile File, MediaClass MediaClass)[] Zip()
        {
            if (MediaClass.Length == 0)
            {
                return this.Media.Select(x => (x, Core.Models.Enums.MediaClass.Unclassified)).ToArray();
            }

            return this.Media.Zip(MediaClass, (f, c) => (f, c)).ToArray();
        }
    }
}