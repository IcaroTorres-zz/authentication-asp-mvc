using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Services
{
    public static class StringRenderer
    {
        /// <summary>
        /// Render a View or partialview as string to general purpose usage
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderView(string viewPath, object model = null, bool partial = true)
        {
            var controller = CreateController<GenericController>();
            var context = controller.ControllerContext;

            // first find the ViewEngine for this view
            var viewEngineResult = partial ? ViewEngines.Engines.FindPartialView(context, viewPath)
                                           : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, controller.ViewData, controller.TempData, sw);
                view.Render(ctx, sw);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Construct a generic controller able to have a controllerContext, navigate views folders and access view templates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static T CreateController<T>(RouteData routeData = null) where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = new T();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(HttpContext.Current);
            else
                throw new InvalidOperationException(
                    "Can't create Controller Context if no " +
                    "active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") &&
                !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                             .ToLower().Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        public class GenericController : Controller { }
    }
}
