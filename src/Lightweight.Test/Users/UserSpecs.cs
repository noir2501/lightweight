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
using NHibernate.Linq;

namespace Lightweight.Tests.Users
{
    [TestFixture]
    public class UserSpecs : SetUp
    {

        UserRepository _userRepository;
        Portal _portal;

        [Test]
        public void IsUserNameAndEmailUnique()
        {
            bool unique = _userRepository.IsUserNameAndEmailUnique("test", "test@test.com", _portal.Tenant.Id);
            Assert.True(unique);
        }

        [TestFixtureSetUp]
        public override void RunBeforeAnyTests()
        {
            base.RunBeforeAnyTests();
            _userRepository = new UserRepository(kernel.Get<IKeyedRepository<Guid, User>>() as Repository<Guid, User>);
            _portal = Lightweight.Business.Providers.Portal.PortalProviderManager.Provider.GetCurrentPortal();
        }

        [TestFixtureTearDown]
        public override void RunAfterAnyTests()
        {
            base.RunAfterAnyTests();
        }
    }
}
