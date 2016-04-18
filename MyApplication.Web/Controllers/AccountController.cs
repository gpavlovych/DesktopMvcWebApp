// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The account controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The account controller.</summary>
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>TODO The _user manager.</summary>
        private readonly IApplicationUserManager _userManager;

        /// <summary>TODO The _sign in manager.</summary>
        private readonly IApplicationSignInManager _signInManager;

        /// <summary>TODO The _authentication manager.</summary>
        private readonly IAuthenticationManager _authenticationManager;

        /// <summary>Initializes a new instance of the <see cref="AccountController"/> class.</summary>
        /// <param name="userManager">TODO The user manager.</param>
        /// <param name="signInManager">TODO The sign in manager.</param>
        /// <param name="authenticationManager">TODO The authentication manager.</param>
        public AccountController(
            IApplicationUserManager userManager, 
            IApplicationSignInManager signInManager, 
            IAuthenticationManager authenticationManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._authenticationManager = authenticationManager;
        }

        // GET: /Account/Login
        /// <summary>TODO The login.</summary>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        // POST: /Account/Login
        /// <summary>TODO The login.</summary>
        /// <param name="model">TODO The model.</param>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result =
                await
                this._signInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction(
                        "SendCode", 
                        new
                            {
                                ReturnUrl = returnUrl, 
                                RememberMe = model.RememberMe
                            });
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
            }
        }

        // GET: /Account/VerifyCode
        /// <summary>TODO The verify code.</summary>
        /// <param name="provider">TODO The provider.</param>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <param name="rememberMe">TODO The remember me.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await this._signInManager.HasBeenVerifiedAsync())
            {
                return this.View("Error");
            }

            return this.View(
                new VerifyCodeViewModel
                    {
                        Provider = provider, 
                        ReturnUrl = returnUrl, 
                        RememberMe = rememberMe
                    });
        }

        // POST: /Account/VerifyCode
        /// <summary>TODO The verify code.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result =
                await
                this._signInManager.TwoFactorSignInAsync(
                    model.Provider, 
                    model.Code, 
                    isPersistent: model.RememberMe, 
                    rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid code.");
                    return this.View(model);
            }
        }

        // GET: /Account/Register
        /// <summary>TODO The register.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        // POST: /Account/Register
        /// <summary>TODO The register.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser
                               {
                                   UserName = model.Email, 
                                   Email = model.Email
                               };
                var result = await this._userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return this.RedirectToAction("Index", "Publications");
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ConfirmEmail
        /// <summary>TODO The confirm email.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="code">TODO The code.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this._userManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        /// <summary>TODO The forgot password.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        // POST: /Account/ForgotPassword
        /// <summary>TODO The forgot password.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.Email);
                if (user == null || !( await this._userManager.IsEmailConfirmedAsync(user.Id) ))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        /// <summary>TODO The forgot password confirmation.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        // GET: /Account/ResetPassword
        /// <summary>TODO The reset password.</summary>
        /// <param name="code">TODO The code.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? this.View("Error") : this.View();
        }

        // POST: /Account/ResetPassword
        /// <summary>TODO The reset password.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this._userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await this._userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        // GET: /Account/ResetPasswordConfirmation
        /// <summary>TODO The reset password confirmation.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        // POST: /Account/ExternalLogin
        /// <summary>TODO The external login.</summary>
        /// <param name="provider">TODO The provider.</param>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(
                provider, 
                this.Url.Action(
                    "ExternalLoginCallback", 
                    "Account", 
                    new
                        {
                            ReturnUrl = returnUrl
                        }));
        }

        // GET: /Account/SendCode
        /// <summary>TODO The send code.</summary>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <param name="rememberMe">TODO The remember me.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await this._signInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return this.View("Error");
            }

            var userFactors = await this._userManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(
                purpose => new SelectListItem
                               {
                                   Text = purpose, 
                                   Value = purpose
                               }).ToList();
            return this.View(
                new SendCodeViewModel
                    {
                        Providers = factorOptions, 
                        ReturnUrl = returnUrl, 
                        RememberMe = rememberMe
                    });
        }

        // POST: /Account/SendCode
        /// <summary>TODO The send code.</summary>
        /// <param name="model">TODO The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            // Generate the token and send it
            if (!await this._signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return this.View("Error");
            }

            return this.RedirectToAction(
                "VerifyCode", 
                new
                    {
                        Provider = model.SelectedProvider, 
                        ReturnUrl = model.ReturnUrl, 
                        RememberMe = model.RememberMe
                    });
        }

        // GET: /Account/ExternalLoginCallback
        /// <summary>TODO The external login callback.</summary>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this._authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this._signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction(
                        "SendCode", 
                        new
                            {
                                ReturnUrl = returnUrl, 
                                RememberMe = false
                            });
                case SignInStatus.Failure:
                default:

                    // If the user does not have an account, then prompt the user to create an account
                    this.ViewBag.ReturnUrl = returnUrl;
                    this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View(
                        "ExternalLoginConfirmation", 
                        new ExternalLoginConfirmationViewModel
                            {
                                Email = loginInfo.Email
                            });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        /// <summary>TODO The external login confirmation.</summary>
        /// <param name="model">TODO The model.</param>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
            ExternalLoginConfirmationViewModel model, 
            string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this._authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new ApplicationUser
                               {
                                   UserName = model.Email, 
                                   Email = model.Email
                               };
                var result = await this._userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        // POST: /Account/LogOff
        /// <summary>TODO The log off.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this._authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Publications");
        }

        // GET: /Account/ExternalLoginFailure
        /// <summary>TODO The external login failure.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        /// <summary>TODO The dispose.</summary>
        /// <param name="disposing">TODO The disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._userManager != null)
                {
                    this._userManager.Dispose();
                }

                if (this._signInManager != null)
                {
                    this._signInManager.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        /// <summary>TODO The xsr f_ key.</summary>
        private const string XSRF_KEY = "XsrfId";

        /// <summary>TODO The add errors.</summary>
        /// <param name="result">TODO The result.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        /// <summary>TODO The redirect to local.</summary>
        /// <param name="returnUrl">TODO The return url.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Publications");
        }

        /// <summary>TODO The challenge result.</summary>
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            /// <summary>Initializes a new instance of the <see cref="ChallengeResult"/> class.</summary>
            /// <param name="provider">TODO The provider.</param>
            /// <param name="redirectUri">TODO The redirect uri.</param>
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            /// <summary>Initializes a new instance of the <see cref="ChallengeResult"/> class.</summary>
            /// <param name="provider">TODO The provider.</param>
            /// <param name="redirectUri">TODO The redirect uri.</param>
            /// <param name="userId">TODO The user id.</param>
            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            /// <summary>Gets or sets the login provider.</summary>
            public string LoginProvider { get; set; }

            /// <summary>Gets or sets the redirect uri.</summary>
            public string RedirectUri { get; set; }

            /// <summary>Gets or sets the user id.</summary>
            public string UserId { get; set; }

            /// <summary>TODO The execute result.</summary>
            /// <param name="context">TODO The context.</param>
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties
                                     {
                                         RedirectUri = this.RedirectUri
                                     };
                if (this.UserId != null)
                {
                    properties.Dictionary[ XSRF_KEY ] = this.UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
            }
        }

        #endregion
    }
}