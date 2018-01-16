using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class UserProfileModel
    {

        public Guid Id { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
    }
}