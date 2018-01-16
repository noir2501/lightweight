using Lightweight.Business.Repository;
using Lightweight.Model.Entities;
using Lightweight.Tests;
using NUnit.Framework;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NHibernate.Linq;
using Lightweight.Business.Providers.Navigation;

namespace Lightweight.Test.Navigation
{
    [TestFixture]
    public class NavigationSpecs : SetUp
    {

        IKeyedRepository<Guid, Page> _pageRepository;
        IKeyedRepository<Guid, PagePermission> _pagePermissionRepository;
        IKeyedRepository<Guid, Role> _roleRepository;
        Portal _portal;
        NavigationProviderBase _navigationProvider;

        List<Page> _deletedItems = new List<Page>();
        List<PagePermission> _deletedPermissions = new List<PagePermission>();
        List<Role> _deletedRoles = new List<Role>();

        [Test]
        public void CanCreatePages()
        {
            int items = 5;
            for (int i = 0; i < items; i++)
            {
                Page page = new Page(_portal.Tenant, "Test page", "", "~/");
                _pageRepository.Save(page);
                _deletedItems.Add(page);
            }

            Console.WriteLine("Created {0} page(s).", items);
        }

        [Test]
        public void CanCreatePagesWithChildren()
        {
            int children = 5;
            Page parent = new Page(_portal.Tenant, "Test Parent nav item", "", "");
            _pageRepository.Save(parent);

            for (int i = 0; i < children; i++)
            {
                Page child = new Page(parent, String.Format("Child {0} for Test Parent nav item", i), "", "~/child");
                _pageRepository.Save(child);
                _deletedItems.Add(child);
            }

            _deletedItems.Add(parent);

            Console.WriteLine("Created one navigation item with {0} children.", children);
        }

        [Test]
        public void CanReadNavigationItems()
        {
            var items = _pageRepository.All().ToList();
            Console.WriteLine("Got {0} navigation items.", items.Count);
        }

        [Test]
        public void CanCreateNavigationWithPermissions()
        {
            int permissions = 3;
            using (TransactionScope ts = new TransactionScope())
            {
                // create a role
                Role role = new Role(_portal.Tenant, "Test role for navigation permission");
                _roleRepository.Save(role);
                _deletedRoles.Add(role);

                // create pages with a permission each
                for (int i = 0; i < permissions; i++)
                {
                    Page page = new Page(_portal.Tenant, "Test navigation item with permissions", "", "~/");
                    _pageRepository.Save(page);
                    _deletedItems.Add(page);

                    PagePermission permission = new PagePermission(page, role, true, true, false);
                    _pagePermissionRepository.Save(permission);
                    _deletedPermissions.Add(permission);
                }

                ts.Complete();
            }
        }

        [Test]
        public void CanReadNavigationForRole()
        {
            int rolesNo = 3;
            int pagesNo = 3;
            int permissionsNo = rolesNo * pagesNo;

            List<Role> roles = new List<Role>();
            List<Page> pages = new List<Page>();

            // create the roles
            for (int r = 0; r < rolesNo; r++)
            {
                string rolename = string.Format("TNP Role {0}", r);

                using (TransactionScope ts = new TransactionScope())
                {
                    // create a role
                    Role role = new Role(_portal.Tenant, rolename);
                    _roleRepository.Save(role);
                    roles.Add(role);
                    _deletedRoles.Add(role);
                    ts.Complete();
                }
            }

            // create the pages
            for (int p = 0; p < pagesNo; p++)
            {
                string pagename = string.Format("TNP Page {0}", p);
                using (TransactionScope ts = new TransactionScope())
                {
                    Page page = new Page(_portal.Tenant, pagename, pagename, "");
                    _pageRepository.Save(page);
                    pages.Add(page);
                    _deletedItems.Add(page);
                    ts.Complete();
                }
            }

            // create permissions
            foreach (var role in roles)
                foreach (var page in pages)
                    using (TransactionScope ts = new TransactionScope())
                    {
                        PagePermission permission = new PagePermission(page, role, true, true, true);
                        _pagePermissionRepository.Save(permission);
                        _deletedPermissions.Add(permission);
                        ts.Complete();
                    }

            // get the navigation with permissions for each role
            foreach (var role in roles)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    List<Page> navigation = _navigationProvider.GetRoleNavigation(role.Name);
                    Console.Write("{0}", role.Name);
                    Console.WriteLine("{0} pages", navigation.Count);
                    foreach (var page in navigation)
                    {
                        Console.WriteLine("{0} permissions for page {1}", page.Permissions.Count, page.Title);
                    }
                    ts.Complete();
                }
            }
        } // todo: check why it does not return correct permissions for page (seems to work if they have been already created before)

        [Test]
        public void CanReadNavigationForUser()
        {
            // todo: same as for role
        }


        [TestFixtureSetUp]
        public override void RunBeforeAnyTests()
        {
            base.RunBeforeAnyTests();
            _pageRepository = kernel.Get<IKeyedRepository<Guid, Page>>();
            _pagePermissionRepository = kernel.Get<IKeyedRepository<Guid, PagePermission>>();
            _roleRepository = kernel.Get<IKeyedRepository<Guid, Role>>();
            _portal = Lightweight.Business.Providers.Portal.PortalProviderManager.Provider.GetCurrentPortal();
            _navigationProvider = Lightweight.Business.Providers.Navigation.NavigationProviderManager.Provider;
        }

        [TestFixtureTearDown]
        public override void RunAfterAnyTests()
        {
            _pagePermissionRepository.Delete(_deletedPermissions);
            _pageRepository.Delete(_deletedItems);
            _roleRepository.Delete(_deletedRoles);

            base.RunAfterAnyTests();
        }
    }
}