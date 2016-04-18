// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddChallengeOnUnauthorizedResult.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The add challenge on unauthorized result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Web.Filters.Results
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>TODO The add challenge on unauthorized result.</summary>
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        /// <summary>Initializes a new instance of the <see cref="AddChallengeOnUnauthorizedResult"/> class.</summary>
        /// <param name="challenge">TODO The challenge.</param>
        /// <param name="innerResult">TODO The inner result.</param>
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            this.Challenge = challenge;
            this.InnerResult = innerResult;
        }

        /// <summary>Gets the challenge.</summary>
        public AuthenticationHeaderValue Challenge { get; }

        /// <summary>Gets the inner result.</summary>
        public IHttpActionResult InnerResult { get; }

        /// <summary>TODO The execute async.</summary>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await this.InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Only add one challenge per authentication scheme.
                if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == this.Challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(this.Challenge);
                }
            }

            return response;
        }
    }
}