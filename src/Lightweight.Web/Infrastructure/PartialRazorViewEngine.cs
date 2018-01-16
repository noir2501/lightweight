using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lightweight.Web.Infrastructure
{
    public class PartialRazorViewEngine : RazorViewEngine
    {
        private static readonly string[] NewPartialViewLocationFormats = new[] 
        { 
             "~/Views/Modules/{1}/{0}.cshtml",
            "~/Views/{1}/Partial/{0}.cshtml",
            "~/Views/Shared/Partial/{0}.cshtml"
        };

        public PartialRazorViewEngine()
        {
            PartialViewLocationFormats = PartialViewLocationFormats.Union(NewPartialViewLocationFormats).ToArray();
        }
    }
}