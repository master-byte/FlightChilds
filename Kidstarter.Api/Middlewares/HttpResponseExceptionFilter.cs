using System.Net;

using Kidstarter.Api.Tools;
using Kidstarter.Core.Exceptions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kidstarter.Api.Middlewares
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is null)
            {
                return;
            }

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<HttpResponseExceptionFilter>>();
            var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

            logger.LogError(context.Exception, "Произошла ошибка");

            var errorText = context.Exception.Message;
            var ex = context.Exception.InnerException;
            while (ex is not null)
            {
                errorText += $"\n---\n{ex.Message}";
                ex = ex.InnerException;
            }

            var errorCode = context.Exception is BaseException baseException ? baseException.ExceptionCode : null;

            var code = context.Exception switch
            {
                NotAuthorizedException _ => HttpStatusCode.Unauthorized,
                EntityNotFoundException _ => HttpStatusCode.NotFound,
                BaseException _ => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (code == HttpStatusCode.InternalServerError && env.IsProduction())
            {
                errorText = "Произошла непредвиденная ошибка";
            }

            context.Result = new ObjectResult(new ApiResponse<string>(errorText) { ErrorCode = errorCode }) { StatusCode = (int)code };

            context.ExceptionHandled = true;
        }
    }
}