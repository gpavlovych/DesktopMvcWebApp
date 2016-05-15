using System;
using System.Threading.Tasks;

namespace MyApplication.Desktop.Services
{
    /// <summary>The HttpClient interface.</summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>Gets string result via GET HTTP request asynchronously.</summary>
        /// <param name="uri">The uri.</param>
        /// <returns>The response.</returns>
        Task<string> GetStringAsync(string uri);
    }
}