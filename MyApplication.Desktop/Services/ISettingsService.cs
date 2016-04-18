// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsService.cs" company="">
//   
// </copyright>
// <summary>
//   The settings service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Desktop.Services
{
    /// <summary>The settings service.</summary>
    public interface ISettingsService
    {
        /// <summary>Gets the web app url.</summary>
        string WebAppUrl { get; }

        /// <summary>Gets or sets the user name.</summary>
        string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        string Password { get; set; }

        /// <summary>Gets or sets a value indicating whether is logged in.</summary>
        bool IsLoggedIn { get; set; }

        /// <summary>Saves user-defined settings.</summary>
        void Save();
    }
}