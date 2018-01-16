using System;

namespace Lightweight.Model.Entities
{
    public class Module : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string Configuration { get; set; }

        public Module(Guid moduleId)
            : this()
        {
            this.Id = moduleId;
        }

        public Module()
        {

        }
    }
}