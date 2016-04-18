// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsService.cs" company="">
//   
// </copyright>
// <summary>
//   The settings service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MyApplication.Desktop.Properties;

namespace MyApplication.Desktop.Services
{
    /// <summary>The settings service.</summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>The application settings.</summary>
        private readonly Settings _settings;

        /// <summary>Initializes a new instance of the <see cref="SettingsService"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public SettingsService(Settings settings)
        {
            this._settings = settings;
        }

        #region ISettingsService Members

        /// <summary>Gets or sets a value indicating whether is logged in.</summary>
        public bool IsLoggedIn
        {
            get
            {
                return this._settings.IsLoggedIn;
            }

            set
            {
                this._settings.IsLoggedIn = value;
            }
        }

        /// <summary>Gets the web app url.</summary>
        public string WebAppUrl
        {
            get
            {
                return this._settings.WebAppUrl;
            }
        }

        /// <summary>Gets or sets the user name.</summary>
        public string UserName
        {
            get
            {
                return this._settings.UserName;
            }

            set
            {
                this._settings.UserName = value;
            }
        }

        /// <summary>Gets or sets the password.</summary>
        public string Password
        {
            get
            {
                return this._settings.Password;
            }

            set
            {
                this._settings.Password = value;
            }
        }

        /// <summary>Saves user-defined settings.</summary>
        /// <remarks>Can't be unit tested, excluded from code coverage.</remarks>
        [ExcludeFromCodeCoverage]
        public void Save()
        {
            this._settings.Save();
        }

        #endregion
    }
}