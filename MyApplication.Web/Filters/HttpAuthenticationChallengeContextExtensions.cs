// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpAuthenticationChallengeContextExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The http authentication challenge context extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Web.Filters
{
    using System;
    using System.Net.Http.Headers;
    using System.Web.Http.Filters;
    using Results;

    /// <summary>TODO The http authentication challenge context extensions.</summary>
    public static class HttpAuthenticationChallengeContextExtensions
    {
        /// <summary>TODO The challenge with.</summary>
        /// <param name="context">TODO The context.</param>
        /// <param name="scheme">TODO The scheme.</param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        /// <summary>TODO The challenge with.</summary>
        /// <param name="context">TODO The context.</param>
        /// <param name="scheme">TODO The scheme.</param>
        /// <param name="parameter">TODO The parameter.</param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        /// <summary>TODO The challenge with.</summary>
        /// <param name="context">TODO The context.</param>
        /// <param name="challenge">TODO The challenge.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }
}