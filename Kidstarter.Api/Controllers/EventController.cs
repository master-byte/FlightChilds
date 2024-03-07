using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Endpoints.Organizer.Events.Models.Create;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Update;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Exceptions;
using Kidstarter.BusinessLogic.Models;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.BusinessLogic.Requests.Parent;
using Kidstarter.BusinessLogic.Requests.Parent.Children;
using Kidstarter.BusinessLogic.Requests.Parent.Events;
using Kidstarter.BusinessLogic.Requests.Parent.Portfolios;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Models.Filters;
using Kidstarter.Core.Services;
using Kidstarter.Infrastructure.Extensions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

using FindEventsRequest = Kidstarter.BusinessLogic.Requests.Organizer.Events.FindEventsRequest;
using GetEventRequest = Kidstarter.BusinessLogic.Requests.Parent.Events.GetEventRequest;

namespace Kidstarter.Api.Controllers
{
    [Route("api/event")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        private readonly IEntityService entityService;

        public EventController(IMapper mapper, IMediator mediator, IEntityService entityService)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.entityService = entityService;
        }

        /// <summary>
        /// Получить весь список кастингов.
        /// </summary>
        [HttpGet("list")]
        [Authorize(Roles = "Продюсер")]
        public async Task<ApiResponse<ListDto<CastingViewModelV1>>> GetAllEvents([FromQuery] Pager pager)
        {
            var result = await this.mediator.Send(
                             new FindEventsRequest(
                                 EventFilter.Empty with
                                 {
                                     Directions = new[] { "Casting" }
                                 },
                                 pager.ToPagedRequest()));

            var eventDetails = result.ChangeType(x => this.mapper.Map<CastingViewModelV1[]>(x.Select(e => e.Event as Casting)));

            return this.ApiOk(new ListDto<CastingViewModelV1>(eventDetails));
        }

        /// <summary>
        /// Создать кастинг.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<CastingViewModelV1>> Create([FromBody] CastingCreateModel createModel)
        {
            var @event = this.mapper.Map<Casting>(createModel, opt => opt.Items["CreatorId"] = this.GetCurrentUser().Id);
            @event.CreatorId = this.GetCurrentUser().Id.ToString();
            var result = await this.mediator.Send(
                             new CreateEventRequest(@event, "Casting"));

            return this.ApiCreated(this.mapper.Map<CastingViewModelV1>(result as Casting));
        }

        /// <summary>
        /// Отметить что пользователь просмотрел кастинг (или продюсер просмотрел откликнувшихся).
        /// </summary>
        [HttpPost("{eventId:guid}/onCheckVisitor")]
        [Authorize(Roles = "Продюсер,Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Obsolete("This method must be split into 2.")]
        public async Task<ApiResponse<bool>> OnCheckVisitor(Guid eventId)
        {
            if (this.User.IsInRole(UserRoleTypeEnum.Producer.GetName()))
            {
                var enventStatusId = eventId;
                await this.mediator.Send(
                             new ViewEventSubscriptionRequest(enventStatusId));
            }
            else
            {
                await this.mediator.Send(new ViewEntityRequest(Identity.Create(eventId)));
            }

            return this.ApiCreated(true);
        }

        #region Producer

        /// <summary>
        /// Список откликнувшихся родителей на кастинг.
        /// </summary>
        [HttpGet("feedbackList")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<FeedbackListViewModelV1>> GetFeedbackUserList(Guid eventId, [FromQuery] Pager pager)
        {
            var subscribers = await this.mediator.Send(
                                  new GetEventSubscribersRequest(
                                      eventId,
                                      pager.ToPagedRequest()));

            var viewModels = subscribers.Entities.Select(
                    x => this.mapper.MapSplitEntitiesIntoUser(
                        this.mapper.Map<UserViewModel>(x.UserStatus?.User),
                        x.UserStatus?.User.Children.FirstOrDefault(c => c.IsDefault),
                        x.FilmingPortfolio))
                .ToArray();

            var result = new FeedbackListViewModelV1
            {
                List = this.mapper.Map<EventSubscribersViewModel[]>(subscribers.Entities.Select(x => x.UserStatus))
            };
            result.List.ForEach(
                x =>
                    {
                        x.User = viewModels.FirstOrDefault(u => u.Id.ToString() == x.UserId);
                    });

            return this.ApiOk(result);
        }

        /// <summary>
        /// Получить кастинг.
        /// </summary>
        [HttpGet("{eventId:guid}")]
        [Authorize(Roles = "Продюсер,Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<CastingViewModelV1>> Get(Guid eventId)
        {
            Casting casting;
            if (this.User.IsInRole(UserRoleTypeEnum.Parent.GetName()))
            {
                var result = await this.mediator.Send(new GetEventRequest(eventId));
                casting = result.Event as Casting;
            }
            else
            {
                var result = await this.mediator.Send(new BusinessLogic.Requests.Organizer.Events.GetEventRequest(eventId));
                casting = result.Event as Casting;
            }

            return this.ApiOk(this.mapper.Map<CastingViewModelV1>(casting));
        }

        [HttpPost]
        [Route("{eventId:guid}/upload")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<UploadViewModel[]>> Upload(Guid eventId, IFormFile[] media)
        {
            if (media.Length == 0)
            {
                return new ApiResponse<UploadViewModel[]>(Array.Empty<UploadViewModel>());
            }

            var uploads = await this.mediator.Send(
                              new UploadEventMediaFileRequest(
                                  eventId,
                                  this.mapper.Map<MediaUploadRequest[]>(media)));

            return this.ApiCreated(this.mapper.Map<UploadViewModel[]>(uploads));
        }

        [HttpDelete]
        [Route("{eventId:guid}/upload/{uploadId:guid}")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<Guid[]>> DeleteUpload(Guid eventId, Guid uploadId)
        {
            var results = await this.mediator.Send(
                              new DeleteEventMediaFileRequest(
                                  eventId,
                                  new[] { Identity.Create(uploadId) }));

            return this.ApiOk(results.Select(x => x.Value).ToArray());
        }

        /// <summary>
        /// Изменить данные кастинга.
        /// </summary>
        [HttpPut("{eventId:guid}")]
        [Authorize(Roles = "Продюсер")]
        public async Task<ApiResponse<CastingViewModelV1>> Update(Guid eventId, [FromBody] CastingUpdateModel updateModel)
        {
            await this.mediator.Send(
                new UpdateEventRequest<Casting>(
                    eventId,
                    evt => this.mapper.Map(updateModel, evt)));

            return this.ApiOk(
                new CastingViewModelV1
                {
                    EventId = eventId,
                });
        }

        [HttpPost]
        [Route("finish/{eventId:guid}")]
        [Authorize(Roles = "Продюсер")]
        public async Task<ApiResponse<CastingViewModelV1>> Finish(Guid eventId)
        {
            await this.mediator.Send(
                new UpdateEventRequest<Event>(
                    eventId,
                    x => x.IsActive = false));

            return this.ApiOk(
                new CastingViewModelV1
                {
                    EventId = eventId,
                });
        }

        /// <summary>
        /// Приглашение на кастинг (продюсер).
        /// </summary>
        [HttpPost("invite")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse> Invite([FromQuery] InviteModel inviteModel)
        {
            var status = await this.entityService.FindOne<EventUserStatus>(
                         x => x.EventId == inviteModel.EventId && x.UserId == inviteModel.UserId.ToString());
            if (status == null)
            {
                return this.ApiError("Отклик не найден");
            }

            await this.mediator.Send(
                new InviteToEventRequest(status.EventUserStatusId));

            return this.ApiCreated(true);
        }

        /// <summary>
        /// Отклонить приглашение на кастинг V2.
        /// </summary>
        [HttpDelete("deleteFeedback")]
        [Authorize(Roles = "Продюсер")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<bool>> DeleteFeedback([FromQuery] Guid feedbackId)
        {
            await this.mediator.Send(
                new RejectEventSubscriptionRequest(feedbackId));

            return this.ApiOk(true);
        }

        #endregion

        #region Parent (Mobile App)

        /// <summary>
        /// Получить отфильтрованный список кастингов.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("Use /parent/events/find instead.")]
        public async Task<ApiResponse<ListDto<CastingViewModelV1>>> Search(
            [FromQuery] CastingFilter query,
            [FromQuery] Pager pager)
        {
            var filter = EventFilter.Empty with
            {

                AgeTo = query.Age,
                Directions = new[]
                  {
                      "Casting"
                  },
                EventDate = query.DateTo,
                Gender = query.Gender,
                Status = EventUserStatusFilter.All
            };

            var events = await this.mediator.Dispatch(
                             new BusinessLogic.Requests.Parent.Events.FindEventsRequest(filter, pager.ToPagedRequest(), EventOrdering.EventDateAsc),
                             res =>
                                 {
                                     var results = this.mapper.Map<CastingViewModelV1[]>(res.Entities.Select(x => x.Event as Casting));
                                     if (query.Type.HasValue && query.Type != CastingType.Undefined)
                                     {
                                         results = results.Where(x => x.Type == query.Type).ToArray();
                                     }

                                     results = results.Join(
                                         res.Entities,
                                         vm => vm.EventId,
                                         e => e.Event.EventId,
                                         (vm, e) =>
                                             {
                                                 vm.State = this.mapper.Map<EventStatusValue>(e.Status);
                                                 return vm;
                                             }).ToArray();

                                     return new ListDto<CastingViewModelV1>(results);
                                 });

            return this.ApiOk(events);
        }

        /// <summary>
        /// Создать отклик на кастинг.
        /// </summary>
        [HttpPost("{eventId:guid}/subscribe")]
        [Authorize(Roles = "Родитель")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<string>> Subscribe(Guid eventId)
        {
            var defaultChild = await this.mediator.Send(new GetDefaultChildRequest());
            if (defaultChild != null)
            {
                var portfolios = await this.mediator.Send(new GetPortfoliosRequest(defaultChild.ChildId));
                var filmingPortfolio = portfolios.FirstOrDefault(x => x.PortfolioType == "Casting");
                if (filmingPortfolio != null)
                {
                    await this.mediator.Send(
                        new SubscribeToEventRequest(eventId, filmingPortfolio.PortfolioId));
                }
                else
                {
                    throw new PortfolioNotFilledException();
                }
            }
            else
            {
                throw new ProfileNotFilledException();
            }

            return this.ApiCreated("ok");
        }

        /// <summary>
        /// Удалить отклик на кастинг.
        /// </summary>
        [HttpDelete("{eventId:guid}/unsubscribe")]
        [Authorize(Roles = "Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("For backwards compatibility. Will be removed.")]
        public async Task<ApiResponse<bool>> Unsubscribe(Guid eventId)
        {
            await this.mediator.Send(
                new UnsubscribeFromEventRequest(eventId));

            return this.ApiOk(true);
        }

        /// <summary>
        /// Список кастингов с приглашениями для родителя.
        /// </summary>
        [HttpGet("invitedList")]
        [Authorize(Roles = "Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("Use /parent/events/find?status=Invited instead.")]
        public async Task<ApiResponse<ListDto<CastingStatusViewModelV1>>> GetInvitedList([FromQuery] Pager pager)
        {
            var filter = EventFilter.Empty with
            {
                Status = EventUserStatusFilter.Invited,
                Directions = new[]
                  {
                      "Casting"
                  }
            };

            var result = await this.mediator.Send(
                             new BusinessLogic.Requests.Parent.Events.FindEventsRequest(
                                 filter,
                                 pager.ToPagedRequest(),
                                 EventOrdering.EventDateAsc));

            var viewModels = result.ChangeType(
                events => events.Select(
                    e => new CastingStatusViewModelV1
                    {
                        Event = this.mapper.Map<CastingViewModelV1>(e.Event),
                    }));
            viewModels.Entities.ForEach(e => e.Event.State = EventStatusValue.Invited);

            return this.ApiOk(new ListDto<CastingStatusViewModelV1>(viewModels));
        }

        [HttpGet("counterUpdatedEvents")]
        [Authorize(Roles = "Родитель")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ApiResponse<int> CounterUpdatedEvents()
        {
            //// int result = await this.mediator.Send(new CounterUpdatedCastingsRequest(this.GetCurrentUser()));

            return this.ApiOk(0);
        }

        #endregion
    }
}
