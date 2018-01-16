using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class RegisterModel
    {
        [Key]
        [ReadOnly(true)]
        public Guid Id;

        [Required(ErrorMessage = "Please enter your user name.")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@[a-z0-9-]+(\\.[a-z0-9-]+)*(\\.[a-z]{2,3})$", ErrorMessage = "Email is not a valid e-mail address.")]
        public string Email { get; set; }

        [Required(ErrorMessage="Please enter your password.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage="Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "User role")]
        public string Role { get; set; }

        public List<System.Web.Mvc.SelectListItem> Roles { get; set; }

        //todo: add roles from database instead of hardcoded values
        public RegisterModel()
        {
            Roles = new List<System.Web.Mvc.SelectListItem>();
            Roles.Add(new System.Web.Mvc.SelectListItem() { Value = "", Text = "No role" });
            Roles.Add(new System.Web.Mvc.SelectListItem() { Value = "0928d77d-4b39-4c72-ab5f-d02afa388c18", Text = "Administrator" });
        }

    }
}