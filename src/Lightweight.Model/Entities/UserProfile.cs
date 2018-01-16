using System;

namespace Lightweight.Model.Entities
{
    public class UserProfile : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }

        public virtual string Address { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Photo { get; set; }
        public virtual string Description { get; set; }

        public virtual DateTime LastUpdated { get; set; }

        public virtual User User { get; set; }

        public UserProfile(User user)
        {
            User = user;
        }

        protected UserProfile()
        {

        }
    }
}
