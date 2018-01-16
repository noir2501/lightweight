using System;

namespace Lightweight.Model.Entities
{
    public class Tenant : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Portal Portal { get; set; }

        protected Tenant()
        {

        }

        public Tenant(Guid tenantId)
        {
            Id = tenantId;
        }
    }
}