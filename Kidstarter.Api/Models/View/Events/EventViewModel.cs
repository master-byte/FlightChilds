using System;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.Base;
using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Models.View.Events
{
    /// <summary>
    /// Base class for different type of events including Casting.
    /// </summary>
    public class EventViewModel : ModelWithMediaBase
    {
        public Guid EventId { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public string Place { get; set; }

        public string City { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }

        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }

        public int? NumberOfSpots { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public bool IsActive { get; set; }

        public DirectionViewModel Direction { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventSubscriptionMechanism SubscriptionMechanism { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string MetroStation { get; set; }
    }
}
