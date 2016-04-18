// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationFailureResult.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The authentication failure result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Web.Filters.Results
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>TODO The authentication failure result.</summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        /// <summary>Initializes a new instance of the <see cref="AuthenticationFailureResult"/> class.</summary>
        /// <param name="reasonPhrase">TODO The reason phrase.</param>
        /// <param name="request">TODO The request.</param>
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            this.ReasonPhrase = reasonPhrase;
            this.Request = request;
        }

        /// <summary>Gets the reason phrase.</summary>
        public string ReasonPhrase { get; }

        /// <summary>Gets the request.</summary>
        public HttpRequestMessage Request { get; }

        /// <summary>TODO The execute async.</summary>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute());
        }

        /// <summary>TODO The execute.</summary>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = this.Request;
            response.ReasonPhrase = this.ReasonPhrase;
            return response;
        }
    }
}
