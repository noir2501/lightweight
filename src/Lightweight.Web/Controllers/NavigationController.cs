using System.Transactions;
using System.Web.Security;
using AutoMapper;
using Lightweight.Business.Repository;
using Lightweight.Model.Entities;
using System.Web.Mvc;
using System;
using Lightweight.Business.Providers.Portal;
using Lightweight.Web.Infrastructure;
using Lightweight.Web.Models;

namespace Lightweight.Web.Controllers
{
    [Authorize]
    public class NavigationController : PortalController
    {
        private readonly IKeyedRepository<Guid, Page> _pageRepository;
        private readonly IKeyedRepository<Guid, Role> _roleRepository;

        public NavigationController
        (
            IKeyedRepository<Guid, Page> pageRepository,
            IKeyedRepository<Guid, Role> roleRepository
        )
        {
            _pageRepository = pageRepository;
            _roleRepository = roleRepository;
        }

        //
        // GET: /Navigation/

        public ActionResult Index()
        {
            return View();
        }

        //public JsonNetResult GetNavigationForRole(string role)
        //{
        //    var model = (role == "anonymous"
        //                     ? NavigationModelHelper.GetDefaultNavigationModel()
        //                     : NavigationModelHelper.GetRoleNavigationModel(role));

        //    return new JsonNetResult(model);
        //}

        //[HttpPost]
        //public JsonNetResult CreateNavigation(NavigationModel model)
        //{
        //    var tenantId = this.Portal.Tenant.Id;

        //    var navigation = Mapper.Map<NavigationModel, Navigation>(model);

        //    navigation.Name = string.Format("{0} navigation", model.Role);
        //    navigation.Tenant = new Tenant(tenantId);

        //    _roleRepository.BeginTransaction();
        //    navigation.Role = _roleRepository.FindBy(r => r.Name == model.Role && r.Tenant.Id == tenantId);
        //    _navigationRepository.Add(navigation);
        //    _roleRepository.CommitTransaction();

        //    Mapper.Map(navigation, model);

        //    return new JsonNetResult(model);
        //}

        //[HttpPost]
        //public JsonNetResult CreateNavigationItem(NavigationItemModel model)
        //{
        //    var item = Mapper.Map<NavigationItemModel, NavigationItem>(model);

        //    using (TransactionScope ts = new TransactionScope())
        //    {
        //        _navigationItemRepository.BeginTransaction();
        //        var unordered = _navigationItemRepository.FilterBy(
        //                ni =>
        //                ni.Navigation.Id == model.NavigationId && ni.Order >= model.Order &&
        //                ni.ParentItem.Id == model.ParentId);

        //        if (unordered != null)
        //            foreach (var uitem in unordered)
        //                uitem.Order++;

        //        _navigationItemRepository.CommitTransaction();

        //        if (unordered != null)
        //            _navigationItemRepository.Update(unordered);
        //        _navigationItemRepository.Insert(item);

        //        ts.Complete();
        //    }

        //    Mapper.Map(item, model);

        //    return new JsonNetResult(model);
        //}

        //[HttpPut]
        //public JsonNetResult UpdateNavigationItem(NavigationItemModel model)
        //{
        //    using (TransactionScope ts = new TransactionScope())
        //    {

        //        var item = _navigationItemRepository.FindById(model.Id);
        //        int oldOrder = item.Order;
        //        int newOrder = model.Order;


        //        dynamic unordered = null;

        //        // if order has changed, update order for items in between
        //        _navigationItemRepository.BeginTransaction();
        //        if (newOrder < oldOrder) // order decreased
        //        {
        //            unordered = _navigationItemRepository.FilterBy(
        //                ni =>
        //                ni.Navigation.Id == model.NavigationId &&
        //                ni.ParentItem.Id == model.ParentId &&
        //                ni.Order >= newOrder && ni.Order < oldOrder
        //                );

        //            if (unordered != null)
        //                foreach (var uitem in unordered)
        //                    uitem.Order++;
        //        }
        //        else if (newOrder > oldOrder) // order increased
        //        {
        //            unordered = _navigationItemRepository.FilterBy(
        //                ni =>
        //                ni.Navigation.Id == model.NavigationId &&
        //                ni.ParentItem.Id == model.ParentId &&
        //                ni.Order > oldOrder && ni.Order <= newOrder
        //                );

        //            if (unordered != null)
        //                foreach (var uitem in unordered)
        //                    uitem.Order--;
        //        }
        //        _navigationItemRepository.CommitTransaction();

        //        if (unordered != null)
        //            _navigationItemRepository.Update(unordered);

        //        Mapper.Map(model, item);
        //        _navigationItemRepository.Update(item);

        //        ts.Complete();
        //    }

        //    return new JsonNetResult(model);
        //}

        //[HttpDelete]
        //public JsonNetResult DeleteNavigationItem(int id)
        //{

        //    using (TransactionScope ts = new TransactionScope())
        //    {
        //        var item = _navigationItemRepository.FindById(id);
        //        var model = Mapper.Map<NavigationItem, NavigationItemModel>(item);

        //        // delete children
        //        _navigationItemRepository.BeginTransaction();
        //        var children = _navigationItemRepository.FilterBy(
        //            ni =>
        //                ni.ParentItem.Id == model.Id);
        //        _navigationItemRepository.CommitTransaction();

        //        _navigationItemRepository.Delete(children);

        //        // update siblings order
        //        _navigationItemRepository.BeginTransaction();
        //        var unordered = _navigationItemRepository.FilterBy(
        //               ni =>
        //               ni.Navigation.Id == model.NavigationId && ni.Order > model.Order &&
        //               ni.ParentItem.Id == model.ParentId);

        //        if (unordered != null)
        //            foreach (var uitem in unordered)
        //                uitem.Order--;

        //        _navigationItemRepository.CommitTransaction();

        //        if (unordered != null)
        //            _navigationItemRepository.Update(unordered);

        //        // delete item
        //        _navigationItemRepository.Delete(item);

        //        ts.Complete();
        //    }

        //    return new JsonNetResult("deleted");
        //}
    }
}