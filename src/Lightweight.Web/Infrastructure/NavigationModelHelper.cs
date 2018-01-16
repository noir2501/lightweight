using System.Collections.Generic;
using AutoMapper;
using Lightweight.Business.Providers.Navigation;
using Lightweight.Model.Entities;
using Lightweight.Web.Models;
using System.Linq;
using System;

namespace Lightweight.Web.Infrastructure
{
    public sealed class NavigationModelHelper
    {
        public static List<PageModel> GetRoleNavigationModel(string rolename)
        {
            List<Page> roleNavigation = NavigationProviderManager.Provider.GetRoleNavigation(rolename);
            List<PageModel> model = Mapper.Map<List<Page>, List<PageModel>>(roleNavigation);

            return model;
        }

        public static List<PageModel> GetUserNavigationModels(string username)
        {
            List<Page> navigation = NavigationProviderManager.Provider.GetUserNavigation(username);
            List<PageModel> model = Mapper.Map<List<Page>, List<PageModel>>(navigation);

            return model;
        }

        public static List<PageModel> GetAdminNavigation(List<PageModel> navigation)
        {
            List<PageModel> adminNavigation = new List<PageModel>(navigation);

            
            var menuOnly = navigation.Where(p => p.MenuOnly);
            foreach (var page in menuOnly)
            {
                // add "edit-page" link under each menu-only page - as first child
                PageModel editLink = new PageModel()
                {
                    Published = true,
                    MenuOnly = true,
                    Name = "Edit",
                    IconUrl = "fa-edit",
                    ParentId = page.Id,
                    Order = -2,
                    Url = string.Format("~/page/{0}/edit", page.Slug)
                };

                adminNavigation.Add(editLink);

                // add "add-page" link under each menu-only page - as last child
                PageModel addLink = new PageModel()
                {
                    Published = true,
                    MenuOnly = true,
                    Name = "Add page",
                    IconUrl = "fa-plus-square-o",
                    ParentId = page.Id,
                    Order = -1,
                    Url = string.Format("~/page/{0}/create", page.Slug)
                };

                adminNavigation.Add(addLink);
            }

            // add "add-page" link under root
            PageModel addRootLink = new PageModel()
            {
                Published = true,
                MenuOnly = true,
                Name = "Add page",
                IconUrl = "fa-plus-square-o",
                Order = 100,
                Url = string.Format("~/page/create", Guid.Empty)
            };

            adminNavigation.Add(addRootLink);

            return adminNavigation;
        }


        public static List<PageModel> MergeNavigationModels(List<PageModel> source, List<PageModel> destination)
        {
            //TODO: for now, just assume the 'source' items do not colide with the 'destination' items, and just add them to root of the navigation
            return source.Union(destination).ToList();
        }
    }
}