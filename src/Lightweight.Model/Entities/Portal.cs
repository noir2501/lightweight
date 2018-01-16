using System;
using System.Collections.Generic;

namespace Lightweight.Model.Entities
{
    public class Portal : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Title { get; set; }
        public virtual string Url { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual IList<PortalAlias> Aliases { get; set; }
    }
}