using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Lightweight.Business.Repository;
using Lightweight.Business.Repository.Entities;
using Lightweight.Model.Entities;
using Lightweight.Web.Infrastructure;
using Lightweight.Web.Models;
using System.Web.Security;
using System.Transactions;
using NHibernate.Linq;

namespace Lightweight.Web.Controllers
{
    [Authorize(Roles = "Member, Administrator")]
    public class UsersController : PortalController
    {

        private readonly IKeyedRepository<Guid, User> _userRepository;
        private readonly IKeyedRepository<Guid, UserProfile> _userProfileRepository;
        private readonly IKeyedRepository<Guid, Role> _roleRepository;

        public UsersController(IKeyedRepository<Guid, User> userRepository, IKeyedRepository<Guid, UserProfile> userProfileRepository, IKeyedRepository<Guid, Role> roleRepository)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _roleRepository = roleRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Edit(RegisterModel model)
        //{
        //    var user = model.Id != default(Guid) ? _userRepository.GetUserById(model.Id, Portal.Tenant.Id) : new User(Guid.Empty);
        //    model = Mapper.Map<User, RegisterModel>(user);

        //    return View(model);
        //}

        //public JsonNetResult GetUsers()
        //{
        //    var users = _userRepository.GetAllUsers(this.Portal.Tenant.Id);
        //    var models = Mapper.Map<List<User>, List<UserModel>>(users);

        //    return new JsonNetResult(new { data = models });
        //}

        /*
        [HttpPost]
        public ActionResult Update(RegisterModel model)
        {
            User user = null;

            // if password is specified passwords must match
            if (!string.IsNullOrEmpty(model.Password) && !string.Equals(model.Password, model.ConfirmPassword))
            {
                Alert(AlertType.warning, "Validation error", "Passwords do not match.");
                return View("Edit", model);
            }

            // user is new...
            if (model.Id == default(Guid))
            {
                // must specify password
                if (string.IsNullOrEmpty(model.Password))
                {
                    Alert(AlertType.warning, "Validation error", "You must specify a password for a new user.");
                    return View("Edit", model);
                }

                // username and email must be unique for this tenant
                bool unique = _userRepository.IsUserNameAndEmailUnique(model.UserName, model.Email, Portal.Tenant.Id);
                if (!unique)
                {
                    Alert(AlertType.warning, "Validation error", "User's name and email must be unique in the system.");
                    return View("Edit", model);
                }

                user = Mapper.Map<User>(model);
                user.Enabled = true; // users created from this page are automatically enabled
            }
            else // read existing user with roles from database
            {
                user = _userRepository.GetUserWithRolesById(model.Id, Portal.Tenant.Id);
                Mapper.Map<RegisterModel, User>(model, user);
            }

            // update user's roles (just one role per user for now)
            Guid roleId; // default for no role, or role's Id
            Guid.TryParse(model.Role, out roleId);
            user.Roles.Clear();
            if (roleId != default(Guid))
                user.Roles.Add(_roleRepository.FindById(roleId));

            // set last updated date
            user.Profile.LastUpdated = DateTime.Now;

            // save/update the user and return model
            try
            {

                if (!string.IsNullOrEmpty(model.Password))
                    user.Hash = ((Lightweight.Business.Providers.Membership.MultiTenantMembershipProvider)Membership.Provider).EncodePassword(model.Password);

                _userRepository.Save(user);
                Mapper.Map(user, model);
            }
            catch (Exception)
            {
                Alert(AlertType.danger, "Error", "Failed to create/update user.");
                return View("Edit", model);
            }

            Alert(AlertType.success, "Success", string.Format("User {0} successfully created/updated.", model.UserName));
            return RedirectToAction("Index");
        }*/

        //public ActionResult Delete(Guid id)
        //{
        //    var user = _userRepository.GetUserById(id, Portal.Tenant.Id);
        //    _userRepository.Delete(user);

        //    return RedirectToAction("Index");
        //}

        /// <summary>
        /// Retrieve user's profile and return the profile page
        /// </summary>
        /// <param name="id">username</param>
        public new ActionResult Profile(string id)
        {
            User user = null;
            UserProfileModel profile = null;

            using (TransactionScope ts = new TransactionScope())
            {
                user = (from u in _userRepository.All()
                        where u.Tenant.Id == Portal.Tenant.Id && u.UserName == id
                        select u)
                         .Fetch(u => u.Profile)
                         .SingleOrDefault();

                ts.Complete();
            }

            profile = Mapper.Map<UserProfileModel>(user.Profile);
            profile.UserName = user.UserName;

            return View(profile);
        }

        [HttpPost]
        [Authorize(Roles = "Member, Administrator")]
        public JsonNetResult UpdateProfile(UserProfileModel model)
        {
            User user = null;

            using (TransactionScope ts = new TransactionScope())
            {
                user = (from u in _userRepository.All()
                        where u.Tenant.Id == Portal.Tenant.Id && u.Id == model.Id
                        select u)
                         .Fetch(u => u.Profile)
                         .SingleOrDefault();

                // only allow editing if the user is editing his own profile or he has Admin role
                if (!User.IsInRole("Administrator") && User.Identity.Name != user.UserName)
                    throw new Lightweight.Business.Exceptions.BusinessException("You do not have sufficient permissions to update this profile.");

                Mapper.Map(model, user.Profile);
                _userProfileRepository.Save(user.Profile);

                ts.Complete();
            }

            return new JsonNetResult(model);
        }
    }
}