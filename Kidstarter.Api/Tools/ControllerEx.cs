using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kidstarter.Api.Tools
{
    public static class ControllerEx
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Ok")]
        public static ApiResponse<object> DefaultApiOk(this Controller _)
        {
            return new ApiResponse<object>();
        }

        public static ApiResponse<TData> ApiCreated<TData>(this ControllerBase controller, TData data)
        {
            controller.HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            return new ApiResponse<TData>(data);
        }

        public static ApiResponse<TData> ApiCreated<TData>(this ControllerBase controller, string location, TData data)
        {
            controller.HttpContext.Response.Headers["Location"] = location;
            controller.HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            return new ApiResponse<TData>(data);
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Ok")]
        public static ApiResponse<TData> ApiOk<TData>(this ControllerBase _, TData data)
        {
            return new ApiResponse<TData>(data);
        }

        public static ApiResponse<TData> ApiNotModified<TData>(this ControllerBase controller, TData data)
        {
            controller.HttpContext.Response.StatusCode = StatusCodes.Status304NotModified;
            return new ApiResponse<TData>(data);
        }

        public static ApiResponse ApiStatusCode(this ControllerBase controller, HttpStatusCode statusCode)
        {
            controller.HttpContext.Response.StatusCode = (int)statusCode;
            return new ApiResponse();
        }

        public static ApiResponse<TData> ApiError<TData>(this ControllerBase controller, string error)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<TData>(error);
        }

        public static ApiResponse<TData> ApiError<TData>(this ControllerBase controller, List<string> errors)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<TData>(errors);
        }

        public static ApiResponse<TData> ApiError<TData>(this ControllerBase controller, Exception ex)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<TData>(ex);
        }

        public static ApiResponse<TData> ApiNotFound<TData>(this ControllerBase controller)
        {
            controller.HttpContext.Response.StatusCode = 404;
            return new ApiResponse<TData>();
        }

        public static ApiResponse<string> ApiError(this ControllerBase controller, string error)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<string>(error);
        }

        public static ApiResponse<string> ApiError(this ControllerBase controller, List<string> errors)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<string>(errors);
        }

        public static ApiResponse<string> ApiError(this ControllerBase controller, Exception ex)
        {
            controller.HttpContext.Response.StatusCode = 400;
            return new ApiResponse<string>(ex);
        }
    }
}
