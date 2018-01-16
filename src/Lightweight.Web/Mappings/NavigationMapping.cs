using AutoMapper;
using Lightweight.Model.Entities;
using Lightweight.Web.Infrastructure;
using Lightweight.Web.Models;

namespace Lightweight.Web.Mappings
{
    public class NavigationMappings : Profile
    {
        protected override void Configure()
        {
            /*
            Mapper
               .CreateMap<Navigation, NavigationModel>()
               .ForMember(m => m.Role, o => o.MapFrom(s => s.Role.Name));

            Mapper
                .CreateMap<NavigationModel, Navigation>()
                .ForMember(m => m.Tenant, o => o.Ignore())
                .ForMember(m => m.Role, o => o.Ignore());

            Mapper
                .CreateMap<NavigationItem, NavigationItemModel>()
                .ForMember(m => m.NavigationId, o => o.MapFrom(s => s.Navigation.Id))
                .ForMember(m => m.ParentId, o => o.MapFrom(s => s.ParentItem.Id))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<NavigationItemModel, NavigationItem>()
                .ConstructUsing(s => new NavigationItem()
                                       {
                                           Navigation = new Navigation() { Id = s.NavigationId },
                                           ParentItem = s.ParentId.HasValue ? new NavigationItem()
                                                                                    {
                                                                                        Id = s.ParentId.Value
                                                                                    } : null
                                       });
             */
        }
    }
}