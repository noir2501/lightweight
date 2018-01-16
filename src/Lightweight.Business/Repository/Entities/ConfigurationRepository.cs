using System.Collections.Generic;
using System.Linq;
using Lightweight.Model.Entities;
using NHibernate;

namespace Lightweight.Business.Repository.Entities
{
    public class ConfigurationRepository : Repository<int, Configuration>
    {
        public ConfigurationRepository(ISession session)
            : base(session)
        {

        }

        public IList<Configuration> GetConfigurations()
        {
            BeginTransaction();

            var q = from configuration in All()
                    select configuration;

            var result = q.ToList();

            CommitTransaction();

            return result;
        }

        public int CreateConfigurationIfNotExists(string configuration)
        {
            BeginTransaction();

            var cfg = (from config in All()
                       where config.Name == configuration
                       select config).SingleOrDefault();
            int id = 0;

            if (cfg == null)
            {
                cfg = new Configuration(configuration);
                id = Add(cfg);
            }

            CommitTransaction();

            return id;
        }
    }
}
