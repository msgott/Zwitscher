using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zwitscher.Attributes
{

    public class AdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleName = context.HttpContext.Session.GetString("RoleName");

            if (roleName != "Administrator")
            {
                context.Result = new RedirectResult("/Zwitscher");
            }

            base.OnActionExecuting(context);
        }
    }

}
