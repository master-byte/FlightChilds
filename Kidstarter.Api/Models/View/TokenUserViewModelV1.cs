using System;
using System.Linq;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;

namespace Kidstarter.Api.Models.View
{
    [Obsolete]
    public sealed class TokenUserViewModelV1 : UserViewModel
    {
        public OrganizationViewModel ProducerCenter { get; set; }

        public string ProducerCenterName => this.ProducerCenter?.Name;

        public string ProducerCenterPhotoId => this.ProducerCenter?.Photos.FirstOrDefault()?.Id;

        public TokenUploadViewModel ProducerCenterPhoto => new TokenUploadViewModel { Upload = this.ProducerCenter?.Photos.FirstOrDefault() };
    }
}