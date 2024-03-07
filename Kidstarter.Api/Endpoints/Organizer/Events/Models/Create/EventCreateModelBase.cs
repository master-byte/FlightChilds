using System;
using System.ComponentModel.DataAnnotations;

using JsonSubTypes;

using Kidstarter.Api.Binders;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Kidstarter.Api.Endpoints.Organizer.Events.Models.Create
{
    [ModelBinder(BinderType = typeof(EventCreateModelBinder))]
    [JsonConverter(typeof(JsonSubtypes), nameof(CategoryId))]
    [JsonSubtypes.KnownSubType(typeof(CastingCreateModel), "Casting")]
    [JsonSubtypes.KnownSubType(typeof(ChildrenDevelopmentCenterEventCreateModel), "ChildrenDevelopmentCenter")]
    [JsonSubtypes.KnownSubType(typeof(CreativityCenterEventCreateModel), "CreativityCenter")]
    [JsonSubtypes.KnownSubType(typeof(DanceEventCreateModel), "Dance")]
    [JsonSubtypes.KnownSubType(typeof(ForeignLanguageEventCreateModel), "ForeignLanguage")]
    [JsonSubtypes.KnownSubType(typeof(ItEventCreateModel), "IT")]
    [JsonSubtypes.KnownSubType(typeof(MusicEventCreateModel), "Music")]
    [JsonSubtypes.KnownSubType(typeof(ScienceEventCreateModel), "Science")]
    [JsonSubtypes.KnownSubType(typeof(SportEventCreateModel), "Sport")]
    [JsonSubtypes.FallBackSubType(typeof(CastingCreateModel))]
    public abstract class EventCreateModelBase
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(5000)]
        public string About { get; set; }

        [MaxLength(250)]
        public string Place { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [Range(0, 24)]
        public int? AgeFrom { get; set; }

        [Range(0, 24)]
        public int? AgeTo { get; set; }

        [Range(1, int.MaxValue)]
        public uint? NumberOfSpots { get; set; }

        public Guid[] UploadIds { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string EventDirectionId { get; set; }

        public string CategoryId { get; set; }

        public Guid? CreatorId { get; set; }
    }
}
