// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchService.cs" company="">
//   
// </copyright>
// <summary>
//   A concrete implementation of sample service for calling web api.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Threading.Tasks;

namespace MyApplication.Desktop.Services
{
    /// <summary>
    ///     A concrete implementation of sample service for calling web api.
    /// </summary>
    /// <seealso cref="MyApplication.Desktop.Services.ISearchService" />
    public class SearchService : ISearchService
    {
        /// <summary>An instance of <see cref="IHttpClient"/>.</summary>
        private readonly IHttpClient _client;

        /// <summary>Initializes a new instance of the <see cref="SearchService"/> class.</summary>
        /// <param name="client">The client.</param>
        public SearchService(IHttpClient client)
        {
            this._client = client;
        }

        #region ISearchService Members

        /// <summary>
        ///     Calls some API.
        /// </summary>
        /// <returns>
        ///     The result of WebApi call.
        /// </returns>
        public async Task<string> CallSomeApi()
        {
            return await this._client.GetStringAsync("api/Some");
        }

        #endregion
    }
}