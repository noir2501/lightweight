using AutoMapper;
using Lightweight.Model.Entities;
using Lightweight.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Mappings
{
    public class UserMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<User, UserModel>()
               .ForMember(m => m.FirstName, o => o.MapFrom(s => s.Profile.FirstName))
               .ForMember(m => m.LastName, o => o.MapFrom(s => s.Profile.LastName));

            Mapper.CreateMap<UserModel, User>()
                .ForMember(m => m.Profile, o => o.MapFrom(s => s))
                .AfterMap((s, d) => d.Profile.User = d);
        }
    }
}