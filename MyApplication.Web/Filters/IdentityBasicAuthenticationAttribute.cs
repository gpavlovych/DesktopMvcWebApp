// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityBasicAuthenticationAttribute.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The identity basic authentication attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Web.Filters
{
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    /// <summary>TODO The identity basic authentication attribute.</summary>
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        /// <summary>TODO The authenticate async.</summary>
        /// <param name="userName">TODO The user name.</param>
        /// <param name="password">TODO The password.</param>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            UserManager<ApplicationUser> userManager = CreateUserManager();

            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, UserManager doesn't support CancellationTokens.
            ApplicationUser user = await userManager.FindAsync(userName, password);

            if (user == null)
            {
                // No user with userName/password exists.
                return null;
            }

            // Create a ClaimsIdentity with all the claims for this user.
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
            ClaimsIdentity identity = await userManager.ClaimsIdentityFactory.CreateAsync(userManager, user, "Basic");
            return new ClaimsPrincipal(identity);
        }

        /// <summary>TODO The create user manager.</summary>
        /// <returns>The <see cref="UserManager"/>.</returns>
        private static UserManager<ApplicationUser> CreateUserManager()
        {
            return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }
    }
}