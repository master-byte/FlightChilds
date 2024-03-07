using System;
using System.ComponentModel.DataAnnotations;

using JsonSubTypes;

using Kidstarter.Api.Binders;
using Kidstarter.Core.Models.Enums;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Kidstarter.Api.Endpoints.Organizer.Events.Models.Update
{
    [ModelBinder(BinderType = typeof(EventUpdateModelBinder))]
    [JsonConverter(typeof(JsonSubtypes), nameof(EventDirectionId))]
    [JsonSubtypes.KnownSubType(typeof(CastingUpdateModel), "Casting")]
    [JsonSubtypes.KnownSubType(typeof(ChildrenDevelopmentCenterEventUpdateModel), "ChildrenDevelopmentCenter")]
    [JsonSubtypes.KnownSubType(typeof(CreativityCenterEventUpdateModel), "CreativityCenter")]
    [JsonSubtypes.KnownSubType(typeof(DanceEventUpdateModel), "Dance")]
    [JsonSubtypes.KnownSubType(typeof(ForeignLanguageEventUpdateModel), "ForeignLanguage")]
    [JsonSubtypes.KnownSubType(typeof(ItEventUpdateModel), "IT")]
    [JsonSubtypes.KnownSubType(typeof(MusicEventUpdateModel), "Music")]
    [JsonSubtypes.KnownSubType(typeof(ScienceEventUpdateModel), "Science")]
    [JsonSubtypes.KnownSubType(typeof(SportEventUpdateModel), "Sport")]
    [JsonSubtypes.FallBackSubType(typeof(CastingUpdateModel))]
    public abstract class EventUpdateModelBase
    {
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(5000)]
        public string About { get; set; }

        [MaxLength(250)]
        public string Place { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        [Range(0, 24)]
        public int? AgeFrom { get; set; }

        [Range(0, 24)]
        public int? AgeTo { get; set; }

        [Range(1, int.MaxValue)]
        public uint? NumberOfSpots { get; set; }

        [Range(typeof(DateTimeOffset), "1/1/2020", "1/1/2100")]
        public DateTimeOffset? EventDate { get; set; }

        public bool? IsActive { get; set; }

        public string EventDirectionId { get; set; }

        public Guid? CreatorId { get; set; }
    }
}
