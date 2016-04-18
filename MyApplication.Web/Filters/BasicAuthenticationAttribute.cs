// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationAttribute.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The basic authentication attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Web.Filters
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using Results;

    /// <summary>TODO The basic authentication attribute.</summary>
    public abstract class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        /// <summary>Gets or sets the realm.</summary>
        public string Realm { get; set; }

        /// <summary>TODO The authenticate async.</summary>
        /// <param name="context">TODO The context.</param>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

            if (userNameAndPasword == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = await this.AuthenticateAsync(userName, password, cancellationToken);

            if (principal == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }
            else
            {
                // Authentication was attempted and succeeded. Set Principal to the authenticated user.
                context.Principal = principal;
            }
        }

        /// <summary>TODO The authenticate async.</summary>
        /// <param name="userName">TODO The user name.</param>
        /// <param name="password">TODO The password.</param>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        protected abstract Task<IPrincipal> AuthenticateAsync(string userName, string password, 
                                                              CancellationToken cancellationToken);

        /// <summary>TODO The extract user name and password.</summary>
        /// <param name="authorizationParameter">TODO The authorization parameter.</param>
        /// <returns>The <see cref="Tuple"/>.</returns>
        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.ASCII;

            // Make a writable copy of the encoding to enable setting a decoder fallback.
            encoding = (Encoding) encoding.Clone();

            // Fail on invalid bytes rather than silently replacing and continuing.
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (string.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            int colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            string userName = decodedCredentials.Substring(0, colonIndex);
            string password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }

        /// <summary>TODO The challenge async.</summary>
        /// <param name="context">TODO The context.</param>
        /// <param name="cancellationToken">TODO The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            this.Challenge(context);
            return Task.FromResult(0);
        }

        /// <summary>TODO The challenge.</summary>
        /// <param name="context">TODO The context.</param>
        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter;

            if (string.IsNullOrEmpty(this.Realm))
            {
                parameter = null;
            }
            else
            {
                // A correct implementation should verify that Realm does not contain a quote character unless properly
                // escaped (precededed by a backslash that is not itself escaped).
                parameter = "realm=\"" + this.Realm + "\"";
            }

            context.ChallengeWith("Basic", parameter);
        }

        /// <summary>Gets a value indicating whether allow multiple.</summary>
        public virtual bool AllowMultiple
        {
            get { return false; }
        }
    }
}