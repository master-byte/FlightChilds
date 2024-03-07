using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Endpoints.Common.Organizations.Models
{
    public class DirectionViewModelV1
    {
        public string ParentId { get; set; }

        public string EventDirectionId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventSubscriptionMechanism SubscriptionMechanism { get; set; }

        public string BackgroundUrl { get; set; }

        public string ForegroundUrl { get; set; }

        public string SmallLogoUrl { get; set; }

        public string LargeLogoUrl { get; set; }

        public string XLargeLogoUrl { get; set; }

        public string Name { get; set; }
    }
}