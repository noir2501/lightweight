using System;
using System.Linq;
using Lightweight.Model.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Transactions;
using Lightweight.Business.Exceptions;

namespace Lightweight.Business.Repository.Entities
{
    public class UserRepository : Repository<Guid, User>
    {
        public UserRepository(ISession session)
            : base(session)
        {

        }

        public UserRepository(Repository<Guid, User> repository)
            : base(repository.Session)
        {

        }

        public string GetUserNameByEmail(string email, Guid tenantId)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Email == email
                    && user.Tenant.Id == tenantId
                    select user.UserName;

            string username = q.SingleOrDefault();

            CommitTransaction();

            return username;
        }

        public User GetUserByName(string username, Guid tenantId)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.UserName == username
                    && user.Tenant.Id == tenantId
                    select user;

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public User GetUserById(Guid userId, Guid tenantId)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Id == userId
                    && user.Tenant.Id == tenantId
                    select user;

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public User GetUserWithRolesById(Guid userId, Guid tenantId)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Id == userId
                    && user.Tenant.Id == tenantId
                    select user;

            q = q.Fetch(u => u.Roles);

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result;
        }

        // checks if there is another user with the same name or email on the specified tenant
        public bool IsUserNameAndEmailUnique(string username, string email, Guid tenantId)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Tenant.Id == tenantId && (user.UserName == username || user.Email == email)
                    select user.Id;

            int found = q.Count();

            CommitTransaction();

            return found == 0;
        }

        // returns a list of all users for the specified tenant
        public List<User> GetAllUsers(Guid tenantId, int? pageIndex = null, int? pageSize = null)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Tenant.Id == tenantId
                    select user;

            if (pageIndex.HasValue && pageSize.HasValue)
            {

                if (pageIndex < 1)
                    pageIndex = 1;

                if (pageSize.Value > 0 && pageSize < int.MaxValue)
                    q = q.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            q = q.Fetch(u => u.Profile);

            var users = q.ToList();

            CommitTransaction();

            return users;
        }

        public UserProfile GetProfileByUserName(string username, Guid tenantId, out Guid userId)
        {
            BeginTransaction();

            var qu = from user in All()
                     where user.UserName == username
                     && user.Tenant.Id == tenantId
                     select user.Id;

            var _userId = qu.SingleOrDefault();

            var qp = from user in All()
                     where user.Id == _userId
                     select user.Profile;

            var profile = qp.SingleOrDefault();

            CommitTransaction();
            userId = _userId;

            return profile;
        }

        public void SaveUser(User user)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var exists = user.Id == default(Guid) && GetUserByName(user.UserName, user.Tenant.Id) != null;

                if (exists)
                    throw new BusinessException("Failed to save user. A user with the same username already exists.");

                Save(user);

                ts.Complete();
            }
        }

        public void SaveProfile(UserProfile profile)
        {
            new Repository<Guid, UserProfile>(_session)
                .Save(profile);
        }

        public void UpdatePassword(string username, Guid tenantId, string p)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BeginTransaction();

                var usr = (from user in All()
                           where user.UserName == username
                           && user.Tenant.Id == tenantId
                           select user).SingleOrDefault();

                CommitTransaction();

                usr.Hash = p;
                Update(usr);

                ts.Complete();
            }
        }
    }
}
