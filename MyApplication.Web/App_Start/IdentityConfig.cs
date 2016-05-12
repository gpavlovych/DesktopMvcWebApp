using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public interface IApplicationUserManager : IDisposable
    {
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

        Task<bool> GetTwoFactorEnabledAsync(string userId);

        Task<string> GetPhoneNumberAsync(string userId);

        Task<bool> IsEmailConfirmedAsync(string userId);

        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login);

        Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled);

        Task<ApplicationUser> FindByIdAsync(string userId);

        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

        Task<IdentityResult> AddPasswordAsync(string userId, string password);

        IIdentityMessageService SmsService { get; set; }

        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token);

        Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber);

        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task<ApplicationUser> FindByNameAsync(string userName);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        Task<IdentityResult> CreateAsync(ApplicationUser user);

        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    [ExcludeFromCodeCoverage]
    public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
    {
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

    public interface IApplicationSignInManager : IDisposable
    {
        Task<bool> SendTwoFactorCodeAsync(string provider);

        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

        Task<string> GetVerifiedUserIdAsync();

        Task<bool> HasBeenVerifiedAsync();

        Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser);

        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
    }

    // Configure the application sign-in manager which is used in this application.
    [ExcludeFromCodeCoverage]
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>, IApplicationSignInManager
    {
        public ApplicationSignInManager(
            ApplicationUserManager userManager, 
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) this.UserManager);
        }

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
