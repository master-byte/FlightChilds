using System;
using System.ComponentModel.DataAnnotations;

using Kidstarter.Core.Models.Enums;

namespace Kidstarter.Api.Endpoints.Organizer.Events.Models.Create
{
    public class CastingCreateModel : EventCreateModelBase
    {
        [MaxLength(1000)]
        public string Comments { get; set; }

        [Range(typeof(DateTime), "1/1/2020", "1/1/2100")]
        public DateTime? CastingDate { get; set; }

        [Range(typeof(DateTime), "1/1/2020", "1/1/2100")]
        public DateTime? FilmingDate { get; set; }

        [Range(typeof(DateTime), "1/1/2020", "1/1/2100")]
        public DateTime? DateFrom { get; set; }

        [Range(typeof(DateTime), "1/1/2020", "1/1/2100")]
        public DateTime? DateTo { get; set; }

        [Range(40, 230)]
        public uint? HeightFrom { get; set; }

        [Range(40, 230)]
        public uint? HeightTo { get; set; }

        [EnumDataType(typeof(FeeType))]
        public FeeType? Fee { get; set; }

        public uint? Payment { get; set; }

        [EnumDataType(typeof(CastingType))]
        public CastingType? Type { get; set; }
    }
}
