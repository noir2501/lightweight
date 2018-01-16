using System;
using System.Collections.Generic;
using System.Linq;
using Lightweight.Model.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Lightweight.Business.Repository.Entities
{
    public class PageRepository : Repository<Guid, Page>
    {
        public PageRepository(ISession session)
            : base(session)
        {

        }

        public List<Page> GetRoleNavigation(string rolename, Guid tenantId)
        {
            BeginTransaction();

            // get selected role id
            var rq = from role in new RoleRepository(_session).All()
                     where role.Tenant.Id == tenantId && role.Name == rolename
                     select role.Id;

            // get all pages that have at least one view permission for selected role
            var nq = from page in All()
                     where page.Tenant.Id == tenantId && page.Permissions.Any(p => p.Role.Id == rq.Single() && p.View == true)
                     select page;

            // fetch all permissions for selected pages
            var result = nq
                .FetchMany(page => page.Permissions)
                .ThenFetch(perm => perm.Role)
                //.Distinct()
                .ToList();

            CommitTransaction();

            return result;
        }

        public List<Page> GetUserNavigations(string username, Guid tenantId)
        {
            BeginTransaction();

            // get all roles for selected user
            var rq = from user in new UserRepository(_session).All()
                     where user.Tenant.Id == tenantId && user.UserName == username
                     from role in user.Roles
                     select role.Id;

            // get all pages that have at least view permission for selected roles
            var nq = from page in All()
                     where page.Tenant.Id == tenantId
                     && page.Permissions.Any(p => rq.Contains(p.Role.Id) && p.View == true)
                     select page;

            var result = nq
                .FetchMany(page => page.Permissions)
                .ThenFetch(perm => perm.Role)
                //.Distinct()
                .ToList();

            CommitTransaction();

            return result;
        }
    }
}