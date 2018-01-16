using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lightweight.Web.Controllers.Modules
{
    public class ContentController : ModuleController
    {
        public override PartialViewResult Render(dynamic model, dynamic config)
        {
            return PartialView("_Render");
        }

        public override PartialViewResult Editor(dynamic config)
        {
            return PartialView("_Editor");
        }
    }
}