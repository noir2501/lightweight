using AutoMapper;
using Lightweight.Model.Entities;
using Lightweight.Web.Models;

namespace Lightweight.Web.Mappings
{
    public class RoleMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Role, RoleModel>();
        }
    }
}