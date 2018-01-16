using NHibernate;
using Ninject.Modules;
using Ninject.Web.Common;
using Lightweight.Business.Repository;

namespace Lightweight.Business.Providers.NHibernateSession
{
    public class NHibernateRepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            Bind<ISession>().ToProvider(new NHibernateSessionProvider());
            Bind(typeof (IKeyedRepository<,>)).To(typeof (Repository<,>));
        }
    }
}