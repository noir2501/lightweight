using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightweight.Model.Entities
{
    public class PagePermission: IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual bool View { get; set; }
        public virtual bool Edit { get; set; }
        public virtual bool Delete { get; set; }

        public virtual Page Page { get; set; }
        public virtual Role Role { get; set; }
        

        protected PagePermission()
        {

        }

        public PagePermission(Page page, Role role, bool view, bool edit = false, bool delete = false)
        {
            Page = page;
            Role = role;
            View = view;
            Edit = edit;
            Delete = delete;
        }

        public virtual void SetPermissionRights(bool view, bool edit, bool delete)
        {
            View = view;
            Edit = edit;
            Delete = delete;
        }
    }
}