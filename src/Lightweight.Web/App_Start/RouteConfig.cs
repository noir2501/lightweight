using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lightweight.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CreatePage",
                url: "page/create",
                defaults: new { controller = "Page", action = "Create" }
            ).DataTokens.Add("RouteName", "CreatePage");

            routes.MapRoute(
                name: "SavePage",
                url: "page/save",
                defaults: new { controller = "Page", action = "Save" }
            ).DataTokens.Add("RouteName", "SavePage");

            routes.MapRoute(
                name: "Page",
                url: "page/{slug}/{action}",
                defaults: new { controller = "Page", action = "Render" }
            ).DataTokens.Add("RouteName", "Page");

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           ).DataTokens.Add("RouteName", "Default");

            routes.MapRoute(
                name: "Modules",
                url: "modules/{controller}/{action}/{id}",
                defaults: new { controller = "Module", action = "Render", id = UrlParameter.Optional }
            ).DataTokens.Add("RouteName", "Modules");

           
        }
    }
}