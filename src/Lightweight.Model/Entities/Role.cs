using System;
using System.Collections.Generic;

namespace Lightweight.Model.Entities
{
    public class Role : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string Icon { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual IList<User> Users { get; set; }

        protected Role()
        {
            Users = new List<User>();
        }

        public Role(Tenant tenant, string name)
            : this()
        {
            Tenant = tenant;
            Name = name;
        }
    }
}