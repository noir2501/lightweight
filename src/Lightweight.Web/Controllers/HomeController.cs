using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lightweight.Web.Controllers
{
    public class HomeController : PortalController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}