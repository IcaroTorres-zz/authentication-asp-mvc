using System.Net;
using System.Web.Mvc;

namespace App.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected void PrepareReturnMessage()
        {
            string message = TempData["Message"] as string;
            if (!string.IsNullOrEmpty(message))
            {
                if (message.ToString().Contains("error"))
                {
                    ViewBag.Error = message.Replace("error: ", "");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    ViewBag.Success = message;
                    Response.StatusCode = (int)HttpStatusCode.Created;
                }
                TempData["Message"] = null;
            }
        }
    }
}