using App.Models;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using static Services.StringRenderer;

namespace App
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BinderConfig.RegisterBinders();
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            if (context.Request.UserLanguages != null && Request.UserLanguages.Length > 0)
            {
                string culture = Request.UserLanguages[0];
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket),
                                                                                              authTicket.UserData.Split(','));
            }
        }

        protected void Application_Error()
        {
            var error = Server.GetLastError();
            if (error != null)
            {
                Response.Clear();
                Response.StatusCode = error is HttpException ? ((HttpException)error).GetHttpCode() : 500;

                var isAjaxRequest = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
                if (isAjaxRequest)
                {

                    //create toast model with config and error message to display
                    var toast = new MessageDisplayModel
                    {
                        Title = $"500 Internal Server Error",
                        Color = "danger",
                        Message = $"Fails on {Request.HttpMethod}. Exception found: {error.Message}."
                    };

                    if (error is HttpException httpException)
                    {
                        Response.StatusCode = httpException.GetHttpCode();

                        //change toast model with config and error message to display
                        toast.Title = $"{Response.StatusCode} {Response.StatusDescription}";
                        toast.Message = $"Fails on {Request.HttpMethod}. Exception found: {httpException.Message}.";
                        error = httpException;
                    }

                    // write the rendered toastView as responseText
                    Response.Write(RenderView("~/Views/MessageDisplay/_Toast.cshtml", toast, true));
                }
                else
                {
                    var handlerInfo = new HandleErrorInfo(error, "Generic", error.TargetSite.DeclaringType.Name);
                    Response.Write(RenderView("~/Views/Shared/Error.cshtml", handlerInfo, false));
                }
                Response.End();
            }
        }
    }
}