using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyApplication.Web.Controllers
{
    [Authorize]
    public class SomeController : Controller
    {
        // GET: Some
        public ActionResult Index()
        {
            return View();
        }
    }
}