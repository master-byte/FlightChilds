using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Models.View.Events;
using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Endpoints.Parent.Events.Models
{
    public class ParentEventDetailsViewModel
    {
        public DirectionViewModel Direction => this.Event?.Direction;

        public EventViewModel Event { get; set; }

        public OrganizationViewModel Organization { get; set; }

        public OrganizerViewModel Organizer { get; set; }

        public int? SpotsLeft { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventStatusValue Status { get; set; }
    }
}