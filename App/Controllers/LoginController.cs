using Domain.Entities;
using App.Models;
using Security;
using Services.Email;
using Services.Email.Models;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StringRenderer = Services.StringRenderer;

namespace App.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly IAuthProvider _auth;
        private readonly IEmailWorker _email;
        private static readonly string toastViewPath = "~/Views/MessageDisplay/_Toast.cshtml";
        private static readonly string invalidHashLinkViewPath = "~/Views/Login/RecoveryNotAllowed.cshtml";
        private static readonly string passwordRecoveryViewPath = "~/Views/Email/PasswordRecoveryEmail.cshtml";
        private static readonly string changePasswordViewPath = "~/Views/Email/ChangePasswordEmail.cshtml";

        public LoginController(IAuthProvider auth, IEmailWorker email)
        {
            _auth = auth;
            _email = email;
        }

        public ActionResult Index()
        {
            PrepareReturnMessage();

            if ((User)Session["User"] != null || User.Identity.IsAuthenticated)
                FormsAuthentication.RedirectFromLoginPage((Session["User"] as User)?.Email ?? User.Identity.Name, false);

            return View(new LoginModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel payload)
        {
            if (!ModelState.IsValid)
                return View(payload);

            try
            {
                var user = _auth.Authenticate(payload.Email, payload.Password);

                Session["User"] = user;
                FormsAuthentication.SetAuthCookie(user.Email, false);
                var authTicket = new FormsAuthenticationTicket(1, user.Email, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(Session.Timeout),
                                                               payload.RememberMe, user.Role.Name);

                var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                Response.StatusCode = (int)HttpStatusCode.OK;
                TempData["Message"] = "Conectado com sucesso.";

                var redirectUrl = FormsAuthentication.GetRedirectUrl(user.Email, false);
                if (!string.IsNullOrEmpty(redirectUrl))
                    return Redirect(redirectUrl);

                return RedirectToAction("Index", "Donations");
            }
            catch (Exception e)
            {
                Session["User"] = null;
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                TempData["Message"] = "error: " + e.Message;
                return View("Index", payload);
            }
        }

        [HttpPost, Auth, ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session["User"] = null;
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            TempData["Message"] = "Desconectado com sucesso.";
            return RedirectToAction("Index");
        }

        public PartialViewResult RecoveryView() => PartialView("_Recovery", new Models.EmailModel());

        [HttpPost, ValidateAntiForgeryToken]
        public PartialViewResult Recovery(Models.EmailModel payload)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView(toastViewPath, new MessageDisplayModel
                {
                    Title = "Email not sent.",
                    Color = "danger",
                    Message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors))
                });
            }

            var EFUser = _auth.GetCredentials(payload.Email);

            if (EFUser == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return PartialView(toastViewPath, new MessageDisplayModel
                {
                    Title = "Email not sent.",
                    Color = "danger",
                    Message = "Email not registered."
                });
            }

            try
            {
                // get new password recovery hash created to user
                var passwordRecoveryHash = _auth.ProvideLinkToRecovery(EFUser);
                // current root url to complete link
                var rootUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                // setup view model to fill email
                var passwordRecoveryModel = new PasswordRecoveryEmailModel
                {
                    RecipientEmail = EFUser.Email,
                    RecipientDisplayName = EFUser.Name,
                    RecoveryLink = $"{rootUrl}/Login/PasswordRecovery?email={EFUser.Email}&verifier={passwordRecoveryHash.HashCode}",
                    ExpireDate = passwordRecoveryHash.ExpirationDate.Value,
                    Site = rootUrl
                };
                // render the view as string to send via SMTP
                var emailContentHtml = StringRenderer.RenderView(passwordRecoveryViewPath, passwordRecoveryModel, false);

                _email.Send(emailContentHtml, passwordRecoveryModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return PartialView(toastViewPath, new MessageDisplayModel
                {
                    Title = "Email not sent.",
                    Color = "danger",
                    Message = e.Message
                });
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return PartialView(toastViewPath, new MessageDisplayModel
            {
                Title = "Email sent.",
                Color = "success",
                Message = "Por favor, verifique sua caixa de e-mail."
            });
        }

        public ActionResult PasswordRecovery(string email, string verifier)
            => _auth.Validate(email, verifier, Hashs.Recovery) ? View(new NotSignedPasswordModel { Email = email, HashCode = verifier })
                                                               : View(invalidHashLinkViewPath);

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditPassword(NotSignedPasswordModel payload)
        {
            if (!ModelState.IsValid)
                return View("PasswordRecovery", payload);

            var isValidHash = _auth.Validate(payload.Email, payload.HashCode, Hashs.Recovery);
            var EFUser = _auth.GetCredentials(payload.Email);

            if (!isValidHash || EFUser == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                // ModelState.AddModelError("", "User not registered.");
                ViewBag.Error = "Incapaz de alterar a senha para o usuário especificado.";
                return View("PasswordRecovery", payload);
            }

            try
            {
                // get new password recovery hash created to user
                var newLoginHash = _auth.ProvideLogin(EFUser, payload.NewPassword);
                var rootUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                // setup view model to fill email
                var changePasswordEmailModel = new ChangePasswordEmailModel
                {
                    RecipientDisplayName = EFUser.Name,
                    RecipientEmail = EFUser.Email,
                    NewPassword = payload.NewPassword,
                    Site = rootUrl
                };
                // render the view as string to send via SMTP
                var emailContentHtml = StringRenderer.RenderView(changePasswordViewPath, changePasswordEmailModel, false);

                _email.Send(emailContentHtml, changePasswordEmailModel);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                ModelState.AddModelError("", "User not registered.");
                ViewBag.Error = "Incapaz de alterar a senha para o usuário especificado.";
                return View("PasswordRecovery", payload);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            TempData["Message"] = "Entre com sua nova senha.";
            return RedirectToAction("Index");
        }
    }
}