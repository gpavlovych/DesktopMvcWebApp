using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MyApplication.Web.Services
{
    [ExcludeFromCodeCoverage]
    public class UserService : IUserService
    {
        private readonly IAuthenticationManager _authenticationManager;

        public UserService(IAuthenticationManager authenticationManager)
        {
            this._authenticationManager = authenticationManager;
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await this._authenticationManager.GetExternalLoginInfoAsync();
        }

        public IEnumerable<AuthenticationDescription> GetExternalAuthenticationTypes()
        {
            return this._authenticationManager.GetExternalAuthenticationTypes();
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string xsrfKey, string userId)
        {
            return await this._authenticationManager.GetExternalLoginInfoAsync(xsrfKey, userId);
        }

        public async Task<bool> TwoFactorBrowserRememberedAsync(string userId)
        {
            return await this._authenticationManager.TwoFactorBrowserRememberedAsync(userId);
        }
    }
}