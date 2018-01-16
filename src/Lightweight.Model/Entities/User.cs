using System;
using System.Collections.Generic;

namespace Lightweight.Model.Entities
{
    public class User : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Hash { get; set; }
        public virtual bool Enabled { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual UserProfile Profile { get; set; }

        public virtual ISet<Role> Roles { get; protected set; }

        protected User()
        {
            Roles = new HashSet<Role>();
        }

        public User(Guid userId)
            : this()
        {
            Id = userId;
        }

        public User(Tenant tenant, string username, string email, string hash)
            : this()
        {
            Tenant = tenant;

            UserName = username;
            Email = email;
            Hash = hash;
        }
    }
}