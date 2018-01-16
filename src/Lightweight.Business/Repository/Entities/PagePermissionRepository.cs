using Lightweight.Model.Entities;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightweight.Business.Repository.Entities
{
    public class PagePermissionRepository :Repository<Guid, PagePermission>
    {
          public PagePermissionRepository(ISession session)
            : base(session)
        {

        }

    }
}
