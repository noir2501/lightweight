using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class LogOnModel
    {
        [Required(ErrorMessage = "Please enter your user name")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage="Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}