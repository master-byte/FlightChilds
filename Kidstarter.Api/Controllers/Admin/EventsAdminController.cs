using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Create;
using Kidstarter.Api.Endpoints.Organizer.Events.Models.Update;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Models;
using Kidstarter.Api.Models.View;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic.Requests.Organizer.Events;
using Kidstarter.Core.Extensions;
using Kidstarter.Core.Identity;
using Kidstarter.Core.Models;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Models.Enums;
using Kidstarter.Core.Models.Filters;
using Kidstarter.Infrastructure.Extensions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Controllers.Admin
{
    [Route("api/admin/events")]
    [ApiController]
    [Authorize(Policy = Policies.Admin)]
    public class EventsAdminController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public EventsAdminController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        /// <summary>
        /// Получить весь список кастингов.
        /// </summary>
        [HttpGet("list")]
        public async Task<ApiResponse<ListDto<CastingViewModelV1>>> GetAllEvents(
            [FromQuery] EventFilter filter,
            [FromQuery] Pager pager)
        {
            var request = new FindEventsRequest(
                filter,
                pager.ToPagedRequest());

            var result = await this.mediator.Send(request);
            var eventDetails = result.ChangeType(this.mapper.Map<CastingViewModelV1[]>);

            return this.ApiOk(new ListDto<CastingViewModelV1>(eventDetails));
        }

        /// <summary>
        /// Список откликнувшихся родителей на кастинг.
        /// </summary>
        [HttpGet("feedbackList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<FeedbackListViewModelV1>> GetFeedbackUserList(Guid eventId, [FromQuery] Pager pager)
        {
            var subscribers = await this.mediator.Send(
                                  new GetEventSubscribersRequest(
                                      eventId,
                                      pager.ToPagedRequest()));

            return this.ApiOk(
                new FeedbackListViewModelV1
                {
                    List = this.mapper.Map<EventSubscribersViewModel[]>(subscribers.Entities)
                });
        }

        /// <summary>
        /// Получить кастинг.
        /// </summary>
        [HttpGet("{eventId:guid}")]
        [Authorize(Roles = "Админ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<CastingViewModelV1>> Get(Guid eventId)
        {
            var @event = await this.mediator.Send(new GetEventRequest(eventId));
            return @event == null
                       ? this.ApiNotFound<CastingViewModelV1>()
                       : this.ApiOk(this.mapper.Map<CastingViewModelV1>(@event.Event as Casting));
        }

        /// <summary>
        /// Создать кастинг.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ApiResponse<CastingViewModelV1>> Create([FromBody] CastingCreateModel createModel)
        {
            if (createModel.CreatorId == null)
            {
                return this.ApiError<CastingViewModelV1>("Обязательный параметр 'CreatorId' не указан");
            }

            var request = new CreateEventRequest(
                this.mapper.Map<Casting>(createModel, opt => opt.Items["CreatorId"] = createModel.CreatorId),
                "Casting");

            var result = await this.mediator.ExecuteAs(
                             request,
                             new UserIdentity(createModel.CreatorId.Value, UserRoleTypeEnum.Producer));

            return this.ApiCreated(this.mapper.Map<CastingViewModelV1>(result as Casting));
        }

        /// <summary>
        /// Изменить данные кастинга.
        /// </summary>
        [HttpPut("{eventId:guid}")]
        public async Task<ApiResponse<CastingViewModelV1>> Update(Guid eventId, [FromBody] CastingUpdateModel updateModel)
        {
            if (updateModel.CreatorId == null)
            {
                return this.ApiError<CastingViewModelV1>("Обязательный параметр 'CreatorId' не указан");
            }

            var request = new UpdateEventRequest<Casting>(
                eventId,
                e => this.mapper.Map(updateModel, e));

            await this.mediator.ExecuteAs(request, new UserIdentity(updateModel.CreatorId.Value, UserRoleTypeEnum.Producer));
            var result = await this.Get(eventId);

            return result;
        }

        [HttpPost]
        [Route("{eventId:guid}/upload")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ApiResponse<Guid[]>> DeleteUpload(Guid eventId, Guid uploadId)
        {
            var results = await this.mediator.Send(
                              new DeleteEventMediaFileRequest(
                                  eventId,
                                  new[] { Identity.Create(uploadId) }));

            return this.ApiOk(results.Select(x => x.Value).ToArray());
        }
    }
}