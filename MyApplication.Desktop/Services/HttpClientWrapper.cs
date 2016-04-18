// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientWrapper.cs" company="">
//   
// </copyright>
// <summary>
//   The http client wrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Desktop.Services
{
    /// <summary>The http client wrapper.</summary>
    /// <remarks>Can't be unit-tested, so excluded from CodeCoverage.</remarks>
    [ExcludeFromCodeCoverage]
    public class HttpClientWrapper : IHttpClient
    {
        /// <summary>An instance of <see cref="HttpClient"/>.</summary>
        private readonly HttpClient _client;

        /// <summary>The settings service.</summary>
        private readonly ISettingsService _settingsService;

        /// <summary>The value indicating whether this instance is already disposed.</summary>
        private bool _disposed;

        /// <summary>Initializes a new instance of the <see cref="HttpClientWrapper"/> class.</summary>
        /// <param name="client">An instance of <see cref="HttpClient"/> to be used.</param>
        /// <param name="settingsService">The settings service.</param>
        public HttpClientWrapper(HttpClient client, ISettingsService settingsService)
        {
            this._client = client;
            this._settingsService = settingsService;
            var byteArray = Encoding.ASCII.GetBytes(settingsService.UserName + ":" + settingsService.Password);
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", 
                Convert.ToBase64String(byteArray));
        }

        #region IHttpClient Members

        /// <summary>
        /// Gets string result via GET HTTP request asynchronously.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <returns>
        /// The response.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public async Task<string> GetStringAsync(string uri)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            return await this._client.GetStringAsync(new Uri(this._settingsService.WebAppUrl + uri));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this._client != null)
                {
                    this._client.Dispose();
                }
            }

            this._disposed = true;
        }
    }
}