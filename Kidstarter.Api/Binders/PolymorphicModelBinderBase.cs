using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Newtonsoft.Json;

namespace Kidstarter.Api.Binders
{
    internal abstract class PolymorphicModelBinderBase<TBindTo> : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var json = await ExtractRequestJson(bindingContext.ActionContext);
            try
            {
                var data = JsonConvert.DeserializeObject<TBindTo>(json);
                bindingContext.Result = ModelBindingResult.Success(data);
            }
            catch
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError(
                    "Ошибка привязки модели",
                    "Одно из указанных свойств модели имеет неожиданное значение");
            }
        }

        private static Task<string> ExtractRequestJson(ActionContext actionContext)
        {
            var content = actionContext.HttpContext.Request.Body;
            return new StreamReader(content).ReadToEndAsync();
        }
    }
}