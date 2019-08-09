using Domain.Entities;
using App.Models;
using Security;
using Services.Email;
using Services.Email.Models;
using System;
using System.Web.Mvc;
using StringRenderer = Services.StringRenderer;

namespace App.Controllers
{
    [Auth]
    public class ProfileController : Controller
    {
        private readonly IAuthProvider _auth;
        private readonly IEmailWorker _email;
        private static readonly string changePasswordViewPath = "~/Views/Email/ChangePasswordEmail.cshtml";

        public ProfileController(IAuthProvider auth, IEmailWorker email)
        {
            _auth = auth;
            _email = email;
        }

        public ActionResult Index()
        {
            var EFUser = (User)Session["User"];
            if (EFUser == null)
            {
                ViewBag.Error = "Your credentials expired.";
                return RedirectToAction("Index", "Login");
            }
            var profile = new ProfileModel
            {
                Name = EFUser.Name,
                Email = EFUser.Email,
                IsAnonymous = EFUser.IsAnonymous
            };
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileModel payload)
        {
            if (!ModelState.IsValid)
                return View(payload);

            var EFUser = (User)Session["User"];
            if (EFUser == null)
            {
                ViewBag.Error = "Unable to update profile, due to user disabled or not found.";
                return View(payload);
            }
            EFUser = _auth.GetCredentials(EFUser.Email);
            EFUser.Name = payload.Name;
            EFUser.Email = payload.Email;
            EFUser.IsAnonymous = payload.IsAnonymous;

            _auth.UpdateCredentials(EFUser, payload.Password);
            Session["User"] = EFUser;

            // setup view model to fill email render the view as string to send via SMTP
            if (!string.IsNullOrEmpty(payload.Password))
            {
                var rootUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                var changePasswordEmailModel = new ChangePasswordEmailModel
                {
                    RecipientDisplayName = EFUser.Name,
                    RecipientEmail = EFUser.Email,
                    NewPassword = payload.Password,
                    Site = rootUrl
                };
                var emailContentHtml = StringRenderer.RenderView(changePasswordViewPath, changePasswordEmailModel, false);
                _email.Send(emailContentHtml, changePasswordEmailModel);
            }

            ViewBag.Success = "Profile updated";
            return View("Index", payload);
        }
    }
}