using System;

using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models.Base;
using Kidstarter.Core.Models.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kidstarter.Api.Endpoints.Parent.Children.Models
{
    public class ChildViewModel : ModelWithMediaBase
    {
        public int? Age => this.BirthDate.GetAge();

        public DateTime? BirthDate { get; set; }

        public Guid ChildId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string FirstName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender? Gender { get; set; }

        public string SecondName { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}