using System;

namespace Kidstarter.Api.Endpoints.Common.Organizations.Models
{
    public class BusinessHoursViewModel
    {
        public TimeSpan CloseTime { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeSpan OpenTime { get; set; }
    }
}