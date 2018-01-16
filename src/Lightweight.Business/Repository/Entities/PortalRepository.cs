using System;
using System.Linq;
using Lightweight.Model.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Lightweight.Business.Repository.Entities
{
    public class PortalRepository : Repository<Guid, Portal>
    {
        public PortalRepository(ISession session)
            : base(session)
        {

        }

        public Portal GetPortalByUrl(string url)
        {
            BeginTransaction();

            var q = from portal in All()
                    where portal.Url == url
                    select portal;

            q = q.Fetch(p => p.Tenant);

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public Portal GetPortalByAliasUrl(string url)
        {
            BeginTransaction();

            var q = from alias in new Repository<int, PortalAlias>(_session).All()
                    where alias.Url == url
                    select alias;
            q = q.Fetch(a => a.Portal).ThenFetch(p => p.Tenant);

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result != null ? result.Portal : null;
        }
    }
}