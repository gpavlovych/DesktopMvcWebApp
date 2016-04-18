// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchService.cs" company="">
//   
// </copyright>
// <summary>
//   Sample service for calling web api.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Threading.Tasks;

namespace MyApplication.Desktop.Services
{
    /// <summary>
    ///     Sample service for calling web api.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        ///     Calls some API.
        /// </summary>
        /// <returns>The result of WebApi call.</returns>
        Task<string> CallSomeApi();
    }
}