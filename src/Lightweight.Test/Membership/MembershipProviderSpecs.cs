using Lightweight.Business.Repository;
using Lightweight.Business.Repository.Entities;
using Lightweight.Model.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Lightweight.Tests.Membership
{
    [TestFixture]
    public class SingleTenantMembershipProviderSpecs : SetUp
    {
        [Test]
        public void CanRegisterUser()
        {
            string username = Guid.NewGuid().ToString();
            string email = string.Format("{0}@test.com", username);
            string password = "test";

            var user = System.Web.Security.Membership.CreateUser(username, password, email);

            _deleteUsers.Add((Guid)user.ProviderUserKey);
            Console.WriteLine("Created user with Key:{0}.", user.ProviderUserKey);

            // todo: check user, profile (members role?)
        }

        [Test]
        public void CanAddUserToRoles()
        {

        }

        [Test]
        public void CanGetUserByName()
        {
            string name = "test";
            var user = System.Web.Security.Membership.GetUser(name);
            if (user == null)
                Console.WriteLine("Got no user with name {0}.", name);
            else
                Console.WriteLine("Got user with name {0}.", user.UserName);
        }

        [Test]
        public void CanGetUserByEmail()
        {
            string email = "test@test.com";

            var username = System.Web.Security.Membership.GetUserNameByEmail(email);

            if (string.IsNullOrEmpty(username))
                Console.WriteLine("Got no user with for email {0}.", email);
            else
                Console.WriteLine("Got user with name {0} for email {1}.", username, email);
        }

        [Test]
        public void CanGetAllUsers()
        {
            var users = System.Web.Security.Membership.GetAllUsers();
            Console.WriteLine("Got {0} user(s).", users.Count);
        }

        [Test]
        public void CanGetAllUsersPaged()
        {
            int records;
            System.Web.Security.Membership.GetAllUsers(2, 2, out records);
            Console.WriteLine("Got {0} user(s).");
        }

        UserRepository _userRepository;
        List<Guid> _deleteUsers = new List<Guid>();

        [TestFixtureSetUp]
        public override void RunBeforeAnyTests()
        {
            base.RunBeforeAnyTests();
            _userRepository = new UserRepository(kernel.Get<IKeyedRepository<Guid, User>>() as Repository<Guid, User>);
        }

        [TestFixtureTearDown]
        public override void RunAfterAnyTests()
        {
            foreach (var userId in _deleteUsers)
            {
                var user = _userRepository.FindById(userId);
                _userRepository.Delete(user);
            }

            base.RunAfterAnyTests();
        }


    }
}
