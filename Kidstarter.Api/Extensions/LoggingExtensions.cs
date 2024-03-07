using System;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.SystemConsole.Themes;

namespace Kidstarter.Api.Extensions
{
    public static class LoggingExtensions
    {
        public static void ConfigureSerilog(this LoggerConfiguration config, IConfiguration configuration)
        {
            var logConfig = config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Filter.ByExcluding("RequestPath like '/metrics%'")
                .Filter.ByExcluding("RequestPath like '/swagger%'")
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", configuration["ApplicationName"])
                .Enrich.WithProperty("source", Environment.MachineName);

            var consoleTemplate = configuration["Logging:ConsoleOutputTemplate"];
            if (!string.IsNullOrEmpty(consoleTemplate))
            {
                logConfig.WriteTo.Console(
                    outputTemplate: consoleTemplate,
                    theme: AnsiConsoleTheme.Literate);
            }

            if (bool.Parse(configuration["Graylog:Enabled"]))
            {
                var graylogHost = configuration["Graylog:Host"];
                var graylogPort = int.Parse(configuration["Graylog:Port"]);

                logConfig.WriteTo.Graylog(
                    new GraylogSinkOptions
                    {
                        HostnameOrAddress = graylogHost,
                        Port = graylogPort,
                        TransportType = TransportType.Udp
                    });
            }
        }
    }
}
