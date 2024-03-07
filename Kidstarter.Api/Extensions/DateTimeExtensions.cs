using System;

namespace Kidstarter.Api.Extensions
{
    internal static class DateTimeExtensions
    {
        public static int? GetAge(this DateTime? birthDate)
        {
            if (birthDate == null)
            {
                return null;
            }

            var totalDays = (DateTime.UtcNow.Date - birthDate.Value).TotalDays;
            return (int)Math.Truncate(totalDays / 365);
        }
    }
}