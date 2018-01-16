using AutoMapper;
using Lightweight.Model.Entities;
using Lightweight.Web.Infrastructure;
using Lightweight.Web.Models;
using System.Linq;

namespace Lightweight.Web.Mappings
{
    public class PageMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Page, PageModel>()
                .ForMember(m => m.ParentId, o => o.MapFrom(s => s.Parent.Id))
                .ForMember(m => m.MembersVisible, o => o.MapFrom(s => s.Permissions.Any(p => p.Role.Name == "Member" && p.View == true)))
                .ForMember(m => m.GuestsVisible, o => o.MapFrom(s => s.Permissions.Any(p => p.Role.Name == "Guest" && p.View == true)))
                .ForMember(m=>m.Widgets, o=>o.Ignore()) // only map widgets when needed
                .IgnoreAllNonExisting();

            Mapper.CreateMap<PageModel, Page>()
                .ForMember(m => m.Permissions, o => o.Ignore())
                .ForMember(m => m.Tenant, o => o.Ignore())
                .ConstructUsing(s => new Page()
                {
                    Parent = s.ParentId.HasValue ? new Page(s.ParentId.Value) : null
                });

            Mapper.CreateMap<PagePermission, PagePermissionModel>();

            Mapper.CreateMap<PageWidget, PageWidgetModel>()
                .ForMember(m => m.ContentHtml, o => o.MapFrom(s => new MarkdownDeep.Markdown().Transform(s.Content)));

            Mapper.CreateMap<PageWidgetModel, PageWidget>()
                .ForMember(m => m.Page, o => o.MapFrom(s => new Page(s.PageId)))
                .ForMember(m => m.Module, o => o.MapFrom(s => new Module(s.ModuleId)));
        }
    }
}