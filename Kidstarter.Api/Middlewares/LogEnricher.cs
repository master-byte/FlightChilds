using Kidstarter.Api.Tools;

using Microsoft.AspNetCore.Http;

using Serilog;

namespace Kidstarter.Api.Middlewares
{
    public class LogEnricher
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var version = httpContext.Request.Headers["version"];
            var platform = httpContext.Request.Headers["platform"];

            diagnosticContext.Set("ClientVersion", version.ToString());
            diagnosticContext.Set("ClientPlatform", platform.ToString());

            var userId = httpContext.User.GetUserGuidId();
            if (userId != null)
            {
                diagnosticContext.Set("userId", userId);
            }
        }
    }
}