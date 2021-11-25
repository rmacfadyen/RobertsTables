using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace RobertsTables.Code.Binders
{
    /// <summary>
    /// The ReturnToUrlModelBinder ensures that a ReturnTo query string
    /// parameter's value is a url that is local to this application.
    /// If we didn't do this then we'd have an "open redirect" which 
    /// can be used by bad actors to trick people into trusting a link
    /// that they ought not trust.
    /// 
    /// Cleverly asp.net core uses dependency injection to create model
    /// binders so in our case we can easily get access to an IUrlHelper
    /// which will do the heavy lifting with the IsLocalUrl method.
    /// </summary>
    public class LocalUrlModelBinder : IModelBinder
    {
        private readonly IUrlHelper _url;

        public LocalUrlModelBinder(IUrlHelper urlHelper)
        {
            _url = urlHelper;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue; // get the value as string

            if (!_url.IsLocalUrl(value))
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(value);
            }

            return Task.CompletedTask;
        }
    }
}
