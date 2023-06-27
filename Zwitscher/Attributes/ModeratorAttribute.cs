using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zwitscher.Attributes
{

    public class ModeratorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleName = context.HttpContext.Session.GetString("RoleName");

            if (roleName != "Administrator" && roleName != "Moderator")
            {
                context.Result = new RedirectResult("/Zwitscher");
            }

            base.OnActionExecuting(context);
        }
    }

}
