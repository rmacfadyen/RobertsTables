using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RobertsTables.Code.Binders;

namespace RobertsTables.Pages
{
    public class DetailsModel : PageModel
    {
        #region Bind properties
        [BindProperty(SupportsGet = true, BinderType = typeof(LocalUrlModelBinder))] public string ReturnToUrl { get; set; }

        #endregion


        public void OnGet()
        {
        }
    }
}
