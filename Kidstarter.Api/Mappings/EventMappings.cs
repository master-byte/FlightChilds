using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using Kidstarter.Api.Endpoints.Organizer.Events.Models;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Create;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Update;
using Kidstarter.Api.Endpoints.Parent.Events.Models;
using Kidstarter.Api.Models.Base;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Models.View.Events;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;

using EventViewModel = Kidstarter.Api.Models.View.Events.EventViewModel;

namespace Kidstarter.Api.Mappings
{
    internal class EventMappings : Profile
    {
        private static readonly Dictionary<CastingType, string> CastingTypesMap = new()
        {
            [CastingType.Clip] = "VideoClip",
            [CastingType.Advertisement] = "Advertising",
            [CastingType.Show] = "FashionShow",
            [CastingType.Movie] = "Movie",
            [CastingType.Dubbing] = "VoiceActing",
            [CastingType.Photo] = "Photo",
            [CastingType.Undefined] = "Casting",
        };

        public EventMappings()
        {
            this.ToEntities();
            this.FromEntities();
        }

        private void FromEntities()
        {
            this.CreateMap<EventStatusBreakdownView, EventStatusBreakdownViewModel>();

            this.CreateMap<EventUserStatus, EventSubscribersViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EventUserStatusId))
                .ForMember(dest => dest.SubscriptionId, opt => opt.MapFrom(src => src.EventUserStatusId));

            this.CreateMap<Event, CastingViewModelV1>()
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload != null).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Videos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)));

            this.CreateMap<Event, EventViewModel>()
                .IncludeAllDerived();

            this.CreateMap<ChildrenDevelopmentCenterEvent, ChildrenDevelopmentCenterEventViewModel>();
            this.CreateMap<CreativityCenterEvent, CreativityCenterEventViewModel>();
            this.CreateMap<DanceEvent, DanceEventViewModel>();
            this.CreateMap<Casting, FilmingEventViewModel>();
            this.CreateMap<ForeignLanguageEvent, ForeignLanguageEventViewModel>();
            this.CreateMap<ItEvent, ItEventViewModel>();
            this.CreateMap<MusicEvent, MusicEventViewModel>();
            this.CreateMap<ScienceEvent, ScienceEventViewModel>();
            this.CreateMap<SportEvent, SportEventViewModel>();

            this.CreateMap<ParentEventDetails, ParentEventDetailsViewModel>()
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Event.Creator))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Event.Creator != null ? src.Event.Creator.Organization : null));

            this.CreateMap<OrganizerEventDetails, OrganizerEventDetailsViewModel>()
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Event.Creator))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Event.Creator != null ? src.Event.Creator.Organization : null));

            this.CreateMap<Event, ModelWithMediaBase>()
                .IncludeAllDerived()
                .ForMember(
                    dest => dest.Videos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsVideo()).Select(x => x.Upload)))
                .ForMember(
                    dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload.IsImage()).Select(x => x.Upload)));

            this.CreateMap<Casting, CastingViewModelV1>()
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Uploads.Where(x => x.Upload != null).Select(x => x.Upload)))
                .ForMember(dest => dest.Producer, opt => opt.MapFrom(src => src.Creator))
                .ForMember(dest => dest.ProducerCenter, opt => opt.MapFrom(src => src.Creator.Organization));

            // TODO [Obsolete] Remove (Used in EventController.Search)
            this.CreateMap<CastingViewModelV1, ParentEventDetails>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State))
                .ReverseMap();

            this.CreateMap<EventViewModel, ParentEventDetails>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src))
                .ReverseMap();

            // TODO [Obsolete] Remove (EventController.List)
            this.CreateMap<Event, CastingViewModelV1>()
                .ForMember(dest => dest.Producer, opt => opt.MapFrom(src => src.Creator))
                .ForMember(dest => dest.ProducerCenter, opt => opt.MapFrom(src => src.Creator.Organization))
                .ForMember(dest => dest.ProducerCenter, opt => opt.MapFrom(src => src.Creator.Organization));

            // TODO [Obsolete] Remove (EventController.List)
            this.CreateMap<CastingViewModelV1, OrganizerEventDetails>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src))
                .ReverseMap();

            this.CreateMap<EventUserStatusValue, EventStatusValue>()
                .ConvertUsing((value, _) =>
                {
                    if ((value & EventUserStatusValue.UserViewed) != 0)
                    {
                        return EventStatusValue.Viewed;
                    }

                    if ((value & EventUserStatusValue.UserSubscribed) != 0)
                    {
                        return EventStatusValue.OnReview;
                    }

                    if ((value & EventUserStatusValue.UserInvited) != 0)
                    {
                        return EventStatusValue.Invited;
                    }

                    return EventStatusValue.New;
                });

            this.CreateMap<EventUserStatusValue, EventSubscriptionStatus>()
                .ConvertUsing((value, _) =>
                {
                    if ((value & EventUserStatusValue.OrganizerViewed) != 0)
                    {
                        return EventSubscriptionStatus.ProducerViewed;
                    }

                    if ((value & EventUserStatusValue.OrganizerNew) != 0)
                    {
                        return EventSubscriptionStatus.New;
                    }

                    if ((value & EventUserStatusValue.OrganizerInvited) != 0)
                    {
                        return EventSubscriptionStatus.Invited;
                    }

                    if ((value & EventUserStatusValue.OrganizerRejected) != 0)
                    {
                        return EventSubscriptionStatus.Rejected;
                    }

                    return EventSubscriptionStatus.New;
                });
        }

        private void ToEntities()
        {
            this.CreateMap<Guid, EventUpload>()
                .ConstructUsing(src => new EventUpload { UploadId = src.ToString() });

            this.CreateMap<EventCreateModelBase, Event>()
                .IncludeAllDerived()
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom((_, _, _, context) => context.Items["CreatorId"]))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(
                    dest => dest.Uploads,
                    opt => opt.MapFrom(src => src.UploadIds));

            this.CreateMap<ChildrenDevelopmentCenterEventUpdateModel, ChildrenDevelopmentCenterEvent>();
            this.CreateMap<CreativityCenterEventCreateModel, CreativityCenterEvent>();
            this.CreateMap<DanceEventCreateModel, DanceEvent>();
            this.CreateMap<ForeignLanguageEventCreateModel, ForeignLanguageEvent>();
            this.CreateMap<ItEventCreateModel, ItEvent>();
            this.CreateMap<MusicEventCreateModel, MusicEvent>();
            this.CreateMap<ScienceEventCreateModel, ScienceEvent>();
            this.CreateMap<SportEventCreateModel, SportEvent>();

            this.CreateMap<CastingCreateModel, Casting>()
                .IncludeAllDerived()
                .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.GetValueOrDefault(FeeType.Undefined)))
                .ForMember(dest => dest.DateTo, opt => opt.MapFrom(src => src.CastingDate ?? src.DateTo))
                .ForMember(dest => dest.CastingDate, opt => opt.MapFrom(src => src.CastingDate ?? src.DateTo))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.CastingDate ?? src.DateTo ?? src.EventDate))
                .ForMember(dest => dest.Uploads, opt => opt.MapFrom(src => src.UploadIds))
                .ForMember(dest => dest.EventDirectionId, opt => opt.MapFrom(src => CastingTypesMap[src.Type ?? CastingType.Undefined]));

            this.CreateMap<EventUpdateModelBase, Event>()
                .ForMember(dest => dest.EventDirectionId, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .ForMember(dest => dest.EventDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
                .AfterMap(
                    (s, d) =>
                        {
                            // mapping nullable value type. We only update target value if the source value is not null and ignore nulls otherwise.
                            d.IsActive = s.IsActive ?? d.IsActive;
                            d.Gender = s.Gender ?? d.Gender;
                        })
                .IncludeAllDerived()
                .ForAllOtherMembers(opts => opts.Condition((_, _, srcMember) => srcMember is not null));

            this.CreateMap<ChildrenDevelopmentCenterEventUpdateModel, ChildrenDevelopmentCenterEvent>();
            this.CreateMap<CreativityCenterEventUpdateModel, CreativityCenterEvent>();
            this.CreateMap<DanceEventUpdateModel, DanceEvent>();
            this.CreateMap<ForeignLanguageEventUpdateModel, ForeignLanguageEvent>();
            this.CreateMap<ItEventUpdateModel, ItEvent>();
            this.CreateMap<MusicEventUpdateModel, MusicEvent>();
            this.CreateMap<ScienceEventUpdateModel, ScienceEvent>();
            this.CreateMap<SportEventUpdateModel, SportEvent>();

            this.CreateMap<CastingUpdateModel, Casting>()
                .ForMember(dest => dest.Fee, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
                .ForMember(dest => dest.DateTo, opt => opt.MapFrom(src => src.CastingDate ?? src.DateTo))
                .ForMember(dest => dest.CastingDate, opt => opt.MapFrom(src => src.CastingDate ?? src.DateTo))
                .AfterMap(
                    (s, d) =>
                        {
                            // mapping nullable value type. We only update target value if the source value is not null and ignore nulls otherwise.
                            d.Fee = s.Fee ?? d.Fee;
                            d.EventDate = s.EventDate ?? s.CastingDate ?? s.DateTo ?? d.EventDate;
                            d.EventDirectionId = s.Type.HasValue ? CastingTypesMap[s.Type ?? CastingType.Undefined] : d.EventDirectionId;
                        })
                .ForAllOtherMembers(opts => opts.Condition((_, _, srcMember) => srcMember is not null));
        }
    }
}