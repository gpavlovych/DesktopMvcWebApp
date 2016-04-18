// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The home controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Web.Mvc;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The home controller.</summary>
    public class HomeController : Controller
    {
        /// <summary>TODO The about.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult About()
        {
            this.ViewBag.Message = "Your application description page.";

            return this.View();
        }

        /// <summary>TODO The contact.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}