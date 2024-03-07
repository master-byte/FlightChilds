using System;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.View.Events;
using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Models.View
{
    [Obsolete]
    public class CastingViewModelV1 : FilmingEventViewModel
    {
        [JsonProperty("images")]
        public UploadViewModel[] Media { get; set; }

        public OrganizationViewModelV1 ProducerCenter { get; set; }

        public UserViewModel Producer { get; set; }

        [Obsolete]
        public EventStatusBreakdownViewModel StatusBreakdown { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventStatusValue State { get; set; }
    }
}