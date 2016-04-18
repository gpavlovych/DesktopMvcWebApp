// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsProvider.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The settings provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using MyApplication.Web.Properties;

namespace MyApplication.Web.DAL
{
    /// <summary>TODO The settings provider.</summary>
    public class SettingsProvider: ISettingsProvider
    {
        /// <summary>TODO The _settings.</summary>
        private readonly Settings _settings;

        /// <summary>Initializes a new instance of the <see cref="SettingsProvider"/> class.</summary>
        /// <param name="settings">TODO The settings.</param>
        public SettingsProvider(Settings settings)
        {
            this._settings = settings;
        }

        /// <summary>Gets the page size.</summary>
        public int PageSize
        {
            get
            {
                return this._settings.PageSize;
            }
        }
    }
}