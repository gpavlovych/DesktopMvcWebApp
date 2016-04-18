// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityConfig.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The email service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace MyApplication.Web
{
    /// <summary>TODO The email service.</summary>
    public class EmailService : IIdentityMessageService
    {
        /// <summary>TODO The send async.</summary>
        /// <param name="message">TODO The message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    /// <summary>TODO The sms service.</summary>
    public class SmsService : IIdentityMessageService
    {
        /// <summary>TODO The send async.</summary>
        /// <param name="message">TODO The message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    /// <summary>TODO The ApplicationUserManager interface.</summary>
    public interface IApplicationUserManager : IDisposable
    {
        /// <summary>TODO The get logins async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

        /// <summary>TODO The get two factor enabled async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> GetTwoFactorEnabledAsync(string userId);

        /// <summary>TODO The get phone number async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<string> GetPhoneNumberAsync(string userId);

        /// <summary>TODO The is email confirmed async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> IsEmailConfirmedAsync(string userId);

        /// <summary>TODO The get valid two factor providers async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

        /// <summary>TODO The reset password async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="token">TODO The token.</param>
        /// <param name="newPassword">TODO The new password.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        /// <summary>TODO The remove login async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="login">TODO The login.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login);

        /// <summary>TODO The set two factor enabled async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="enabled">TODO The enabled.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled);

        /// <summary>TODO The find by id async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApplicationUser> FindByIdAsync(string userId);

        /// <summary>TODO The generate change phone number token async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="phoneNumber">TODO The phone number.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

        /// <summary>TODO The add password async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="password">TODO The password.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> AddPasswordAsync(string userId, string password);

        /// <summary>Gets or sets the sms service.</summary>
        IIdentityMessageService SmsService { get; set; }

        /// <summary>TODO The change phone number async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="phoneNumber">TODO The phone number.</param>
        /// <param name="token">TODO The token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token);

        /// <summary>TODO The set phone number async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="phoneNumber">TODO The phone number.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber);

        /// <summary>TODO The change password async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="currentPassword">TODO The current password.</param>
        /// <param name="newPassword">TODO The new password.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>TODO The find by name async.</summary>
        /// <param name="userName">TODO The user name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApplicationUser> FindByNameAsync(string userName);

        /// <summary>TODO The confirm email async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="token">TODO The token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        /// <summary>TODO The create async.</summary>
        /// <param name="user">TODO The user.</param>
        /// <param name="password">TODO The password.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        /// <summary>TODO The create async.</summary>
        /// <param name="user">TODO The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> CreateAsync(ApplicationUser user);

        /// <summary>TODO The add login async.</summary>
        /// <param name="userId">TODO The user id.</param>
        /// <param name="login">TODO The login.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    /// <summary>TODO The application user manager.</summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
    {
        /// <summary>Initializes a new instance of the <see cref="ApplicationUserManager"/> class.</summary>
        /// <param name="store">TODO The store.</param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
                                     {
                                         AllowOnlyAlphanumericUserNames = false, 
                                         RequireUniqueEmail = true
                                     };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
                                         {
                                             RequiredLength = 6, 
                                             RequireNonLetterOrDigit = false, 
                                             RequireDigit = false, 
                                             RequireLowercase = false, 
                                             RequireUppercase = false, 
                                         };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider(
                "Phone Code", 
                new PhoneNumberTokenProvider<ApplicationUser> { MessageFormat = "Your security code is {0}" });
            this.RegisterTwoFactorProvider(
                "Email Code", 
                new EmailTokenProvider<ApplicationUser>
                    {
                        Subject = "Security Code", 
                        BodyFormat = "Your security code is {0}"
                    });
            this.EmailService = new EmailService();
            this.SmsService = new SmsService();

            var dataProtectionProvider = Startup.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                IDataProtector dataProtector = dataProtectionProvider.Create("ASP.NET Identity");

                this.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);
            }
        }
    }

    /// <summary>TODO The ApplicationSignInManager interface.</summary>
    public interface IApplicationSignInManager : IDisposable
    {
        /// <summary>TODO The send two factor code async.</summary>
        /// <param name="provider">TODO The provider.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> SendTwoFactorCodeAsync(string provider);

        /// <summary>TODO The external sign in async.</summary>
        /// <param name="loginInfo">TODO The login info.</param>
        /// <param name="isPersistent">TODO The is persistent.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

        /// <summary>TODO The get verified user id async.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<string> GetVerifiedUserIdAsync();

        /// <summary>TODO The has been verified async.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> HasBeenVerifiedAsync();

        /// <summary>TODO The sign in async.</summary>
        /// <param name="user">TODO The user.</param>
        /// <param name="isPersistent">TODO The is persistent.</param>
        /// <param name="rememberBrowser">TODO The remember browser.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser);

        /// <summary>TODO The two factor sign in async.</summary>
        /// <param name="provider">TODO The provider.</param>
        /// <param name="code">TODO The code.</param>
        /// <param name="isPersistent">TODO The is persistent.</param>
        /// <param name="rememberBrowser">TODO The remember browser.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

        /// <summary>TODO The password sign in async.</summary>
        /// <param name="userName">TODO The user name.</param>
        /// <param name="password">TODO The password.</param>
        /// <param name="isPersistent">TODO The is persistent.</param>
        /// <param name="shouldLockout">TODO The should lockout.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
    }

    // Configure the application sign-in manager which is used in this application.
    /// <summary>TODO The application sign in manager.</summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>, IApplicationSignInManager
    {
        /// <summary>Initializes a new instance of the <see cref="ApplicationSignInManager"/> class.</summary>
        /// <param name="userManager">TODO The user manager.</param>
        /// <param name="authenticationManager">TODO The authentication manager.</param>
        public ApplicationSignInManager(
            ApplicationUserManager userManager, 
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        /// <summary>TODO The create user identity async.</summary>
        /// <param name="user">TODO The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) this.UserManager);
        }

        /// <summary>TODO The create.</summary>
        /// <param name="options">TODO The options.</param>
        /// <param name="context">TODO The context.</param>
        /// <returns>The <see cref="ApplicationSignInManager"/>.</returns>
        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options, 
            IOwinContext context)
        {
            return new ApplicationSignInManager(
                context.GetUserManager<ApplicationUserManager>(), 
                context.Authentication);
        }
    }
}
