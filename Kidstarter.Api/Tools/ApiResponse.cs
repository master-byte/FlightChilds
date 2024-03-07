using System;
using System.Collections.Generic;

using Kidstarter.Core.Constants.Logging;

namespace Kidstarter.Api.Tools
{
    public class ApiResponse
    {
        public object Data { get; set; }

        public IEnumerable<string> ErrorMessage { get; set; }

        public WellKnownExceptionCode? ErrorCode { get; set; }

        public DateTime ServerTimeUtc => DateTime.UtcNow;
    }

    public class ApiResponse<TData> : ApiResponse
    {
        public ApiResponse(TData data)
        {
            this.Data = data;
        }

        public ApiResponse()
        {
        }

        public ApiResponse(string error)
        {
            this.ErrorMessage = new[] { error };
        }

        public ApiResponse(IEnumerable<string> errors)
        {
            this.ErrorMessage = errors;
        }

        public ApiResponse(Exception error)
        {
            this.ErrorMessage = new[] { error.Message };
        }

        public new TData Data { get; }
    }
}