using AutoMapper;
using Lightweight.Model.Entities;
using Lightweight.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Mappings
{
    public class ModuleMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Module, ModuleModel>();
        }
    }
}