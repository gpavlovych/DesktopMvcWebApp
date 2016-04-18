// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageController.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The manage controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The manage controller.</summary>
    [Authorize]
    public class ManageController : Controller
    {
        /// <summary>TODO The _sign in manager.</summary>
        private IApplicationSignInManager _signInManager;

        /// <summary>TODO The _user manager.</summary>
        private IApplicationUserManager _userManager;

        /// <summary>Initializes a new instance of the <see cref="ManageController"/> class.</summary>
        /// <param name="userManager">TODO The user manager.</param>
        /// <param name="signInManager">TODO The sign in manager.</param>
        public ManageController(IApplicationUserManager userManager, IApplicationSignInManager signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        // GET: /Manage/Index
        /// <summary>TODO The index.</summary>
        /// <param name="message">TODO The message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage = message == ManageMessageId.ChangePasswordSuccess
                                             ? "Your password has been changed."
                                             : message == ManageMessageId.SetPasswordSuccess
                                                   ? "Your password has been set."
                                                   : message == ManageMessageId.SetTwoFactorSuccess
                                                         ? "Your two-factor authentication provider has been set."
                                                         : message == ManageMessageId.Error
                                                               ? "An error has occurred."
                                                               : message == ManageMessageId.AddPhoneSuccess
                                                                     ? "Your phone number was added."
                                                                     : message == ManageMessageId.RemovePhoneSuccess
                                                                           ? "Your phone number was removed."
                                                                           : string.Empty;

            var userId = this.User.Identity.GetUserId();
            var model = new IndexViewModel
                            {
                                HasPassword = await this.HasPassword(), 
                                PhoneNumber = await this._userManager.GetPhoneNumberAsync(userId), 
                                TwoFactor = await this._userManager.GetTwoFactorEnabledAsync(userId), 
                                Logins = await this._userManager.GetLoginsAsync(userId), 
                                BrowserRemembered =
                                    await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
                            };
            return this.View(model);
        }

        // POST: /Manage/RemoveLogin
        /// <summary>TODO The remove login.</summary>
        /// <param name="loginProvider">TODO The login provider.</param>
        /// <param name="providerKey">TODO The provider key.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result =
                await
                this._userManager.RemoveLoginAsync(
                    this.User.Identity.GetUserId(), 
                    new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return this.RedirectToAction(
                "ManageLogins", 
                new
                    {
                        Message = message
                    });
        }

        // GET: /Manage/AddPhoneNumber
        /// <summary>TODO The add phone number.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult AddPhoneNumber()
        {
            return this.View();
        }

        // POST: /Manage/AddPhoneNumber
        /// <summary>TODO The add phone number.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // Generate the token and send it
            var code =
                await
                this._userManager.GenerateChangePhoneNumberTokenAsync(this.User.Identity.GetUserId(), model.Number);
            if (this._userManager.SmsService != null)
            {
                var message = new IdentityMessage
                                  {
                                      Destination = model.Number, 
                                      Body = "Your security code is: " + code
                                  };
                await this._userManager.SmsService.SendAsync(message);
            }

            return this.RedirectToAction(
                "VerifyPhoneNumber", 
                new
                    {
                        PhoneNumber = model.Number
                    });
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        /// <summary>TODO The enable two factor authentication.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await this._userManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), true);
            var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        /// <summary>TODO The disable two factor authentication.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await this._userManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), false);
            var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        // GET: /Manage/VerifyPhoneNumber
        /// <summary>TODO The verify phone number.</summary>
        /// <param name="phoneNumber">TODO The phone number.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code =
                await this._userManager.GenerateChangePhoneNumberTokenAsync(this.User.Identity.GetUserId(), phoneNumber);

            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null
                       ? this.View("Error")
                       : this.View(
                           new VerifyPhoneNumberViewModel
                               {
                                   PhoneNumber = phoneNumber
                               });
        }

        // POST: /Manage/VerifyPhoneNumber
        /// <summary>TODO The verify phone number.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this._userManager.ChangePhoneNumberAsync(this.User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.RedirectToAction(
                    "Index", 
                    new
                        {
                            Message = ManageMessageId.AddPhoneSuccess
                        });
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError(string.Empty, "Failed to verify phone");
            return this.View(model);
        }

        // GET: /Manage/RemovePhoneNumber
        /// <summary>TODO The remove phone number.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await this._userManager.SetPhoneNumberAsync(this.User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return this.RedirectToAction(
                    "Index", 
                    new
                        {
                            Message = ManageMessageId.Error
                        });
            }

            var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.RedirectToAction(
                "Index", 
                new
                    {
                        Message = ManageMessageId.RemovePhoneSuccess
                    });
        }

        // GET: /Manage/ChangePassword
        /// <summary>TODO The change password.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        // POST: /Manage/ChangePassword
        /// <summary>TODO The change password.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this._userManager.ChangePasswordAsync(
                    this.User.Identity.GetUserId(), 
                    model.OldPassword, 
                    model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.RedirectToAction(
                    "Index", 
                    new
                        {
                            Message = ManageMessageId.ChangePasswordSuccess
                        });
            }

            this.AddErrors(result);
            return this.View(model);
        }

        // GET: /Manage/SetPassword
        /// <summary>TODO The set password.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult SetPassword()
        {
            return this.View();
        }

        // POST: /Manage/SetPassword
        /// <summary>TODO The set password.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this._userManager.AddPasswordAsync(this.User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
                    if (user != null)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }

                    return this.RedirectToAction(
                        "Index", 
                        new
                            {
                                Message = ManageMessageId.SetPasswordSuccess
                            });
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Manage/ManageLogins
        /// <summary>TODO The manage logins.</summary>
        /// <param name="message">TODO The message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage = message == ManageMessageId.RemoveLoginSuccess
                                             ? "The external login was removed."
                                             : message == ManageMessageId.Error
                                                   ? "An error has occurred."
                                                   : string.Empty;
            var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user == null)
            {
                return this.View("Error");
            }

            var userLogins = await this._userManager.GetLoginsAsync(this.User.Identity.GetUserId());
            var otherLogins =
                this.AuthenticationManager.GetExternalAuthenticationTypes()
                    .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                    .ToList();
            this.ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(
                new ManageLoginsViewModel
                    {
                        CurrentLogins = userLogins, 
                        OtherLogins = otherLogins
                    });
        }

        // POST: /Manage/LinkLogin
        /// <summary>TODO The link login.</summary>
        /// <param name="provider">TODO The provider.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(
                provider, 
                this.Url.Action("LinkLoginCallback", "Manage"), 
                this.User.Identity.GetUserId());
        }

        // GET: /Manage/LinkLoginCallback
        /// <summary>TODO The link login callback.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo =
                await this.AuthenticationManager.GetExternalLoginInfoAsync(XSRF_KEY, this.User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction(
                    "ManageLogins", 
                    new
                        {
                            Message = ManageMessageId.Error
                        });
            }

            var result = await this._userManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                       ? this.RedirectToAction("ManageLogins")
                       : this.RedirectToAction(
                           "ManageLogins", 
                           new
                               {
                                   Message = ManageMessageId.Error
                               });
        }

        /// <summary>TODO The dispose.</summary>
        /// <param name="disposing">TODO The disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this._userManager != null)
            {
                this._userManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        /// <summary>TODO The xsr f_ key.</summary>
        private const string XSRF_KEY = "XsrfId";

        /// <summary>Gets the authentication manager.</summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.HttpContext.GetOwinContext().Authentication;
            }
        }

        /// <summary>TODO The add errors.</summary>
        /// <param name="result">TODO The result.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        /// <summary>TODO The has password.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<bool> HasPassword()
        {
            var user = await this._userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }

            return false;
        }

        /// <summary>TODO The manage message id.</summary>
        public enum ManageMessageId
        {
            /// <summary>TODO The add phone success.</summary>
            AddPhoneSuccess, 

            /// <summary>TODO The change password success.</summary>
            ChangePasswordSuccess, 

            /// <summary>TODO The set two factor success.</summary>
            SetTwoFactorSuccess, 

            /// <summary>TODO The set password success.</summary>
            SetPasswordSuccess, 

            /// <summary>TODO The remove login success.</summary>
            RemoveLoginSuccess, 

            /// <summary>TODO The remove phone success.</summary>
            RemovePhoneSuccess, 

            /// <summary>TODO The error.</summary>
            Error
        }

        #endregion
    }
}