using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Zwitscher.Attributes
{
    public class UserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleName = context.HttpContext.Session.GetString("RoleName");

            if (roleName != "Administrator" && roleName != "Moderator" && roleName != "User")
            {
                context.Result = new RedirectResult("/Zwitscher");
            }

            base.OnActionExecuting(context);
        }
    }

}
