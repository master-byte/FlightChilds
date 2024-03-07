using System;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Models.View.Events
{
    public class FilmingEventViewModel : EventViewModel
    {
        public Guid Id => this.EventId;

        public int UniqueValue { get; set; }

        public string Comments { get; set; }

        public int? Payment { get; set; }

        public DateTime? CastingDate { get; set; }

        public DateTime? FilmingDate { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public CastingType Type { get; set; }

        public int? HeightFrom { get; set; }

        public int? HeightTo { get; set; }

        public FeeType Fee { get; set; }
    }
}
