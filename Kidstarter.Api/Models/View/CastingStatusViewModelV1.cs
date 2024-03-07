using System;
using System.Linq;

using Kidstarter.Api.Endpoints.Common.Organizations.Models;

namespace Kidstarter.Api.Models.View
{
    [Obsolete("Used in EventsController.GetInvitedList")]
    public class CastingStatusViewModelV1
    {
        public CastingViewModelV1 Event { get; set; }

        public string ProducerCenterName => this.ProducerCenter?.Name;

        public string ProducerCenterPhotoId => this.ProducerCenter?.Photos.FirstOrDefault()?.Id;

        public UploadViewModel ProducerCenterPhoto => this.ProducerCenter?.Photos.FirstOrDefault();

        public OrganizationViewModelV1 ProducerCenter => Event?.ProducerCenter;

        public UserViewModel Producer => Event?.Producer;
    }
}