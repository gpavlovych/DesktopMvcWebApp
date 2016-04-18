// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The filter config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Web.Mvc;

namespace MyApplication.Web
{
    /// <summary>TODO The filter config.</summary>
    public class FilterConfig
    {
        /// <summary>TODO The register global filters.</summary>
        /// <param name="filters">TODO The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
