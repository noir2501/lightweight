using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lightweight.Web.Controllers
{
    public abstract class ModuleController : PortalController
    {
        public abstract PartialViewResult Render(dynamic model, dynamic config);
        public abstract PartialViewResult Editor(dynamic config);
    }
}