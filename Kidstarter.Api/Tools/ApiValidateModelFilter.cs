using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kidstarter.Api.Tools
{
    public class ApiValidateModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ModelState.IsValid)
            {
                return;
            }

            var errList = GetModelStateErrors(actionContext.ModelState);
            var errorResponse = new ApiResponse<object>(errList);

            actionContext.Result = new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.BadRequest };

            base.OnActionExecuting(actionContext);
        }

        private static List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            return modelState
                .SelectMany(x => x.Value.Errors)
                .Select(
                    err =>
                        {
                            if (!string.IsNullOrWhiteSpace(err.ErrorMessage))
                            {
                                return err.ErrorMessage;
                            }

                            return !string.IsNullOrWhiteSpace(err.Exception?.Message)
                                       ? err.Exception.Message
                                       : "No error message extracted";
                        })
                .ToList();
        }
    }
}