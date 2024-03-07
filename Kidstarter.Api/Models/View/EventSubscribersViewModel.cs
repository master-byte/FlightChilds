using System;

using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Models.View
{
    public sealed class EventSubscribersViewModel
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public string UserId { get; set; }

        public string EventId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("state")]
        public EventSubscriptionStatus Status { get; set; }

        public UserViewModel User { get; set; }
    }
}