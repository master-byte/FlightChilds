using System;

using Kidstarter.Api.Models.Base;

namespace Kidstarter.Api.Models.View
{
    public class OrganizerViewModel : ModelWithMediaBase
    {
        public Guid Id { get; set; }

        public int UniqueValue { get; set; }

        public string UserValueName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string MetroStation { get; set; }

        public string AdditionalPhoneNumber { get; set; }
    }
}
