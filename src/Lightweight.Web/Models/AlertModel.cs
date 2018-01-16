using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public enum AlertType
    {
        info, success, warning, danger
    }

    public class AlertModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }
    }
}