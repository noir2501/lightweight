using Lightweight.Business.Exceptions;
using Lightweight.Business.Providers.Portal;
using Lightweight.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lightweight.Web.Controllers
{
    public abstract class PortalController : Controller
    {
        /// <summary>
        /// Returns the logged-in user Id. 
        /// Returns -1 if no user is logged in or an error occurs while retrieving the logged in user's Id.
        /// </summary>
        protected Guid LoggedInUserId //TODO: make this singleton
        {
            get
            {
                try
                {
                    var loggedInUser = System.Web.Security.Membership.GetUser(User.Identity.Name);

                    return loggedInUser != null && loggedInUser.ProviderUserKey != null
                               ? (Guid)loggedInUser.ProviderUserKey
                               : Guid.Empty;
                }
                catch (ArgumentException)
                {
                    return Guid.Empty;
                }
            }
        }
        public Model.Entities.Portal Portal
        {
            get
            {
                var portal = PortalProviderManager.Provider.GetCurrentPortal();
                if (portal == null)
                    throw new BusinessException("There is no portal defined for the current domain.");

                return portal;
            }
        }

        //protected string LogEntry(string message, Exception exception, string source, string type, int code)
        //{
        //    string user = HttpContext.User.Identity.Name;

        //    return LoggingProviderManager.Provider.LogEntry(user, source, type, code, message, exception);
        //}

        //protected string LogEntry(string message, Exception exception)
        //{
        //    var source = string.Empty;
        //    var type = string.Empty;
        //    const int code = 0;

        //    return LogEntry(message, exception, source, type, code);
        //}

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Portal = Portal;

            if (Portal == null)
                filterContext.Result = RedirectToAction("Index", "Error");

            base.OnActionExecuting(filterContext);
        }

        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        protected AlertModel Alert(AlertType type, string title, string message)
        {
            var alert = new AlertModel()
            {
                Type = type,
                Title = title,
                Message = message
            };

            TempData["Alert"] = alert;

            return alert;
        }
    }
}