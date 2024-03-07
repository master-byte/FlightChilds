using System;

namespace Kidstarter.Api.Configuration
{
    public class ApplicationSettings
    {
        public string[] PhoneNumbersWhitelist { get; set; } = Array.Empty<string>();

        public string Host { get; set; }

        public Guid ApiToken { get; set; }
    }
}