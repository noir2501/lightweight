using AutoMapper;
using Lightweight.Business.Exceptions;
using Lightweight.Business.Repository;
using Lightweight.Model.Entities;
using Lightweight.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using Lightweight.Web.Infrastructure;

namespace Lightweight.Web.Controllers
{
    public class PageController : PortalController
    {
        private readonly IKeyedRepository<Guid, Page> _pageRepository;
        private readonly IKeyedRepository<Guid, PagePermission> _pagePermissionRepository;
        private readonly IKeyedRepository<Guid, PageWidget> _pageWidgetRepository;
        private readonly IKeyedRepository<Guid, Module> _moduleRepository;
        private readonly IKeyedRepository<Guid, User> _userRepository;
        private readonly IKeyedRepository<Guid, Role> _roleRepository;

        public PageController(
            IKeyedRepository<Guid, Page> pageRepository,
            IKeyedRepository<Guid, PageWidget> pageWidgetRepository,
            IKeyedRepository<Guid, Module> moduleRepository,
            IKeyedRepository<Guid, PagePermission> pagePermissionRepository,
            IKeyedRepository<Guid, User> userRepository,
            IKeyedRepository<Guid, Role> roleRepository
        )
        {
            _pageRepository = pageRepository;
            _pageWidgetRepository = pageWidgetRepository;
            _moduleRepository = moduleRepository;
            _pagePermissionRepository = pagePermissionRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        #region page actions: render, create, edit, layout

        // returns the page view in render mode
        public ActionResult Render(string slug)
        {
            PageModel model = null;

            using (TransactionScope ts = new TransactionScope())
            {
                model = GetPageModel(slug);
                var widgets = _pageWidgetRepository.FilterBy(w => w.Page.Id == model.Id).ToList();
                model.Widgets = Mapper.Map<List<PageWidgetModel>>(widgets).OrderBy(r => r.Row).ThenBy(c => c.Col).ToList();
                model.Mode = PageMode.Render;

                ts.Complete();
            }

            return View("Page", model);
        }

        // returns the page view in create mode
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(string slug)
        {
            PageModel parentModel = null;

            if (!string.IsNullOrEmpty(slug))
                parentModel = GetPageModel(slug); // parent page

            PageModel model = new PageModel()
            {
                ParentId = parentModel != null ? parentModel.Id : default(Guid?),
                Mode = PageMode.Create,
                Slug = parentModel != null ? parentModel.Slug + "-" : string.Empty
            };

            ModelState.Clear();
            return View("Page", model);
        }

        // returns the page view in edit mode
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string slug)
        {
            PageModel model = GetPageModel(slug);
            model.Mode = PageMode.Edit;

            return View("Page", model);
        }

        // returns the page view in layout mode
        [Authorize(Roles = "Administrator")]
        public ActionResult Layout(string slug)
        {
            PageModel model = null;
            using (TransactionScope ts = new TransactionScope())
            {
                model = GetPageModel(slug);
                model.Mode = PageMode.Layout;

                var modules = Mapper.Map<List<ModuleModel>>(_moduleRepository.All().ToList());

                ts.Complete();
                ViewBag.modules = modules;
            }

            return View("Page", model);
        }

        #endregion

        #region page create / edit / remove

        /// Create / update a page, layout, and basic permissions
        [HttpPost]
        public ActionResult Save(PageModel model)
        {
            Page page = null;
            Role adminRole, memberRole, guestRole;
            bool isNew = model.Id == Guid.Empty;

            if (isNew) // if page is new create it
            {
                page = new Page(new Tenant(Portal.Tenant.Id));
                if (model.ParentId.HasValue)
                    page.Parent = new Page(model.ParentId.Value);
            }

            using (TransactionScope ts = new TransactionScope())
            {
                // check if slug is unique
                var anotherPageWithSameSlug = _pageRepository.FindBy(p => p.Tenant.Id == Portal.Tenant.Id && p.Slug == model.Slug && p.Id != model.Id) != null;
                if (anotherPageWithSameSlug == true)
                {
                    Alert(AlertType.warning, "Slug not unique", "Another page with this slug already exists.");
                    return View("Page", model);
                }

                // if page exists, retrieve page and permissions
                if (!isNew)
                {
                    page = (from p in _pageRepository.All()
                            where p.Id == model.Id
                            select p).FetchMany(p => p.Permissions).Single();
                }

                // retrieve system roles
                adminRole = _roleRepository.FindBy(r => r.Tenant.Id == Portal.Tenant.Id && r.Name == "Administrator");
                memberRole = _roleRepository.FindBy(r => r.Tenant.Id == Portal.Tenant.Id && r.Name == "Member");
                guestRole = _roleRepository.FindBy(r => r.Tenant.Id == Portal.Tenant.Id && r.Name == "Guest");

                ts.Complete();
            }

            // update page model with new data
            Mapper.Map<PageModel, Page>(model, page);

            try
            {
                /* add / update page permissions */
                var permissions = page.Permissions;

                // add / update admin permission
                PagePermission adminPermission = permissions.SingleOrDefault(p => p.Role.Name == adminRole.Name);
                if (adminPermission == null)
                    permissions.Add(new PagePermission(page, adminRole, true, true, true));
                else adminPermission.SetPermissionRights(true, true, true);

                // add / update member permission
                PagePermission memberPermission = permissions.SingleOrDefault(p => p.Role.Name == memberRole.Name);
                if (memberPermission == null && model.MembersVisible)
                    permissions.Add(new PagePermission(page, memberRole, true, false, false));
                else if (memberPermission != null)
                    memberPermission.SetPermissionRights(model.MembersVisible, false, false);

                // add / update guest permission
                PagePermission guestPermission = permissions.SingleOrDefault(p => p.Role.Name == guestRole.Name);
                if (guestPermission == null && model.GuestsVisible)
                    permissions.Add(new PagePermission(page, guestRole, true, false, false));
                else if (guestPermission != null)
                    guestPermission.SetPermissionRights(model.GuestsVisible, false, false);

                // save the page with permissions
                using (TransactionScope ts = new TransactionScope())
                {
                    _pageRepository.Save(page); //save the page

                    foreach (var permission in permissions)
                        _pagePermissionRepository.Save(permission);

                    ts.Complete();
                }

                model = Mapper.Map<PageModel>(page); //todo: reads permissions without transaction
            }
            catch
            {
                Alert(AlertType.danger, "Error", "Failed to create/update page.");
                return View("Page", model);
            }

            Alert(AlertType.success, "Success", "Page successfully created/updated.");
            return RedirectToAction("edit", "page", new { slug = model.Slug });
        }

        // Remove a page and every related entities
        public ActionResult Remove(string slug)
        {
            PageModel model = GetPageModel(slug);

            bool hasRemovePermission = false;

            // check if he is in a role that has delete permissions

            using (TransactionScope ts = new TransactionScope())
            {
                var deleteRoles = (from p in model.Permissions
                                   where p.Delete == true
                                   select p.RoleName).ToList();

                var userRoles = from r in
                                    (
                                        from u in _userRepository.All()
                                        where u.Id == LoggedInUserId
                                        select u
                                    ).Fetch(u => u.Roles).Single().Roles
                                select r.Name;

                ts.Complete();

                hasRemovePermission = userRoles.Any(r => deleteRoles.Contains(r));
            }

            if (hasRemovePermission)
                try
                {
                    _pageRepository.Delete(_pageRepository.FindById(model.Id));
                }
                catch
                {
                    Alert(AlertType.danger, "Error", "Failed to remove page. Please remove all its children first.");
                    return RedirectToAction("edit", "page", new { slug = slug });
                }

            Alert(AlertType.success, "Success", string.Format("Page '{0}' successfully removed.", model.Name));
            return Redirect("~/");
        }

        // finds a page by slug for the current tenant
        private PageModel GetPageModel(string slug)
        {
            Page page = null;
            using (TransactionScope ts = new TransactionScope())
            {
                var query = (from p in _pageRepository.All()
                             where p.Tenant.Id == Portal.Tenant.Id && p.Slug == slug
                             select p);

                query = query.FetchMany(p => p.Permissions)
                             .ThenFetch(perm => perm.Role);

                page = query.SingleOrDefault();

                ts.Complete();
            }

            if (page == null)
                throw new BusinessException("The specified page does not exist");

            PageModel model = Mapper.Map<PageModel>(page);

            return model;
        }

        #endregion

        #region page layout and widgets

        // returns the list of available modules
        [HttpGet]
        public JsonNetResult GetModulesList()
        {
            List<Module> modules = _moduleRepository
                .BeginTransaction().All()
                .ToList();

            _moduleRepository.CommitTransaction();

            List<ModuleModel> models = Mapper.Map<List<ModuleModel>>(modules);

            return new JsonNetResult(models);
        }

        // returns a list of widgets for the specified page
        [HttpGet]
        public JsonNetResult GetPageWidgetsList(Guid pageId)
        {
            List<PageWidget> widgets = _pageWidgetRepository
                .BeginTransaction().All()
                .Where(w => w.Page.Id == pageId)
                .ToList();

            _pageWidgetRepository.CommitTransaction();

            List<PageWidgetModel> models = Mapper.Map<List<PageWidgetModel>>(widgets);

            return new JsonNetResult(models);
        }

        [HttpPost]
        public JsonNetResult CreatePageWidget(PageWidgetModel model)
        {
            PageWidget widget = Mapper.Map<PageWidget>(model);
            _pageWidgetRepository.Insert(widget);
            Mapper.Map(widget, model);

            return new JsonNetResult(model);
        }

        [HttpPost] // patch for shared server compatibility, usually would use [HttpPut]
        public JsonNetResult UpdatePageWidget(PageWidgetModel model)
        {
            PageWidget widget = Mapper.Map<PageWidget>(model);
            _pageWidgetRepository.Update(widget);
            Mapper.Map(widget, model);

            return new JsonNetResult(model);
        }

        [HttpPost] // patch for shared server compatibility, usually would use [HttpDelete]
        public JsonNetResult DeletePageWidget(PageWidgetModel model)
        {
            _pageWidgetRepository.Delete(
            _pageWidgetRepository.FindById(model.Id));
            return new JsonNetResult(model);
        }

        #endregion

    }
}