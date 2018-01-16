using System;
using System.Collections.Generic;
using System.Linq;
using Lightweight.Model.Entities;
using NHibernate;

namespace Lightweight.Business.Repository.Entities
{
    public class RoleRepository : Repository<Guid, Role>
    {
        public RoleRepository(ISession session)
            : base(session)
        {

        }

        public IList<string> GetUserRoles(string username, Guid tenantId)
        {
            BeginTransaction();
            var q = from user in new UserRepository(_session).All()
                    where user.UserName == username 
                    && user.Tenant.Id == tenantId
                    from role in user.Roles
                    select role.Name;

            var roles = q.ToList();
            CommitTransaction();

            return roles;
        }

        public bool IsUserInRole(string username, Guid tenantId, string rolename)
        {
            BeginTransaction();
            var uq = from role in All()
                     where role.Name == rolename
                     && role.Tenant.Id == tenantId
                     from user in role.Users
                     where user.UserName == username
                     && user.Tenant.Id == tenantId
                     select user.Id;

            var uid = uq.SingleOrDefault();
            CommitTransaction();

            return uid != default(Guid);
        }

        public IList<string> GetAllRoles(Guid tenantId)
        {
            BeginTransaction();
            var rq = from role in All()
                     where role.Tenant.Id == tenantId
                     select role.Name;

            var roles = rq.ToList();
            CommitTransaction();

            return roles;
        }

        public bool RoleExists(string rolename, Guid tenantId)
        {
            BeginTransaction();
            var rq = from role in All()
                     where role.Name == rolename
                     && role.Tenant.Id == tenantId
                     select role.Id;

            var rid = rq.SingleOrDefault();
            CommitTransaction();

            return rid != default(Guid);
        }

        public void CreateRole(string rolename, Guid tenantId)
        {
            Role r = new Role(new Tenant(tenantId), rolename);
            Insert(r);
        }

        public bool DeleteRole(string rolename, Guid tenantId)
        {
            throw new NotImplementedException();
        }
    }
}