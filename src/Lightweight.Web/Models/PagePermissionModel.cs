using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class PagePermissionModel
    {
        public Guid Id { get; set; }

        public bool View { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }

        public Guid PageId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}