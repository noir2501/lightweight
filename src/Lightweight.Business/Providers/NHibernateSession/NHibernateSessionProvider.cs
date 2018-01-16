using System;
using NHibernate;
using NHibernate.Context;
using Ninject.Activation;

namespace Lightweight.Business.Providers.NHibernateSession
{
    public class NHibernateSessionProvider : Provider<ISession>, IDisposable
    {
        private static NHibernate.Cfg.Configuration _configuration = null;
        public static NHibernate.Cfg.Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = BuildConfiguration();
                return _configuration;
            }
        }

        internal static NHibernate.Cfg.Configuration BuildConfiguration()
        {
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();

            return config;
        }

        internal static ISessionFactory BuildSessionFactory()
        {
            return Configuration.BuildSessionFactory();
        }

        private static readonly object _providerlock = new object();

        /// <summary>
        ///TODO: make this thread safe singleton - http://csharpindepth.com/Articles/General/Singleton.aspx
        /// </summary>
        private static ISessionFactory _sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                lock (_providerlock)
                {
                    if (_sessionFactory == null)
                    {
#if DEBUG
                         HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
#endif
                        _sessionFactory = BuildSessionFactory();
                    }
                }
                return _sessionFactory;
            }
        }

        /// <summary>
        ///TODO: cduta: make this thread safe singleton 
        ///http://csharpindepth.com/Articles/General/Singleton.aspx
        /// </summary>
        private static NHibernateSessionProvider _instance = null;
        public static NHibernateSessionProvider Instance
        {
            get { return _instance ?? (_instance = new NHibernateSessionProvider()); }
        }

        public ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))
                    CurrentSessionContext.Bind(SessionFactory.OpenSession());

                return SessionFactory.GetCurrentSession();
            }
        }

        public ISession OpenSession(bool bind)
        {
            ISession session = SessionFactory.OpenSession();
            if (bind)
                CurrentSessionContext.Bind(session);

            return session;
        }

        public IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public void Dispose()
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                throw new InvalidOperationException("Invalid current session");
            }

            ISession session = _sessionFactory.GetCurrentSession();
            
            CurrentSessionContext.Unbind(_sessionFactory);

            if (!session.IsOpen)
            {
                throw new InvalidOperationException("Session closed before disposing context");
            }

            if (null != session.Transaction && session.Transaction.IsActive)
            {
                if (session.Transaction.WasCommitted == false && session.Transaction.WasRolledBack == false)
                    session.Transaction.Rollback();
            }

            session.Close();
        }

        protected override ISession CreateInstance(IContext context)
        {
           return CurrentSession;
        }
    }
}
