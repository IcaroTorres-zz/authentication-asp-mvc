using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace App
{
    public class AuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["User"] == null) return false;
            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                filterContext.HttpContext.Session["User"] = null;
                filterContext.Result = new RedirectResult("~/Login/");
                return;
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}