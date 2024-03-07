using Kidstarter.Api.Models.View;

namespace Kidstarter.Api.Endpoints.Admin.Events.Models
{
    public class AdminEventDetailsViewModel
    {
        public EventViewModel Event { get; set; }

        public OrganizerViewModel Organizer { get; set; }
    }
}