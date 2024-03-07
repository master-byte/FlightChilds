using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kidstarter.Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<RequestResponseLoggingMiddleware> logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // do not log request/response when uploading files
            if ((context.Request.Path.Value ?? string.Empty).Contains("upload"))
            {
                await this.next(context);
                return;
            }

            context.Request.EnableBuffering();

            await this.FormatRequest(context.Request);

            var originalBody = context.Response.Body;

            try
            {
                await using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                await this.next(context);
                memStream.Position = 0;
                var responseBody = await new StreamReader(memStream).ReadToEndAsync();
                this.logger.LogInformation("{responseBody}", responseBody);
                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private async Task FormatRequest(HttpRequest request)
        {
            using var bodyStreamReader = new StreamReader(request.Body, leaveOpen: true);
            var bodyAsText = await bodyStreamReader.ReadToEndAsync();
            request.Body.Position = 0;

            this.logger.LogInformation(
                "{requestPath} {requestQueryString} {requestBody}",
                request.Path,
                request.QueryString.Value,
                bodyAsText);
        }
    }
}