using Kidstarter.Api.Endpoints.Common.Organizations.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Models.View.Events;

namespace Kidstarter.Api.Endpoints.Organizer.Events.Models
{
    public class OrganizerEventDetailsViewModel
    {
        public DirectionViewModel Direction => this.Event?.Direction;

        public EventViewModel Event { get; set; }

        public OrganizationViewModel Organization { get; set; }

        public OrganizerViewModel Organizer { get; set; }

        public EventStatusBreakdownViewModel StatusBreakdown { get; set; }

        public int? SpotsLeft { get; set; }
    }
}