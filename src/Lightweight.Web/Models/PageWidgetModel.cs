using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class PageWidgetModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public int Col { get; set; }
        public int Row { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Content { get; set; }
        public string ContentHtml { get; set; }

        public Guid PageId { get; set; }
        public Guid ModuleId { get; set; }
    }
}