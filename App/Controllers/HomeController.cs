using System.Web.Mvc;

namespace App.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Profile");
        }
    }
}
