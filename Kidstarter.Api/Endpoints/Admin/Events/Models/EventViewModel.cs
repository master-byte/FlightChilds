using System;

namespace Kidstarter.Api.Endpoints.Admin.Events.Models
{
    public class EventViewModel
    {
        public Guid EventId { get; set; }

        public int EntityId { get; set; }

        public string Name { get; set; }

        public string EventType { get; set; }

        public string EventCategory { get; set; }

        public string OrganizationName { get; set; }

        public string PartnerName { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public bool IsActive { get; set; }
    }
}