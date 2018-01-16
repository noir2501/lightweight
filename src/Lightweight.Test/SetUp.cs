using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightweight.Tests
{
    public class SetUp
    {
        protected StandardKernel kernel = null;

        [TestFixtureSetUp]
        public virtual void RunBeforeAnyTests()
        {
            kernel = new StandardKernel(new NinjectSettings() { InjectNonPublic = true });
            kernel.Load(new Lightweight.Business.Providers.NHibernateSession.NHibernateRepositoryModule());
            kernel.Inject(System.Web.Security.Membership.Provider);
            Console.WriteLine("IOC Kernel initialized...\n");
        }

        [TestFixtureTearDown]
        public virtual void RunAfterAnyTests()
        {
            // nothing for now
            Console.WriteLine("Cleanup completed.");
        }
    }
}
