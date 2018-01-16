using System.Web.Profile;
using AutoMapper;
using Lightweight.Web.Models;
using Lightweight.Model.Entities;

namespace Lightweight.Web.Mappings
{
    public class UserProfileMapping : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ProfileBase, UserProfileModel>()
                  .ForAllMembers(m => m.ResolveUsing<ProfileValueResolver>());

            Mapper.CreateMap<UserModel, UserProfile>();
            //.ForMember(m => m.Id, o => o.Ignore());

            Mapper.CreateMap<UserProfile, UserProfileModel>()

                .ForMember(m => m.UserName, o => o.Ignore());

            Mapper.CreateMap<UserProfileModel, UserProfile>()
                 .ForMember(m => m.Photo, o => o.Ignore())
                 .ForMember(m => m.User, o => o.Ignore());
        }

        public class ProfileValueResolver : IValueResolver
        {
            public ResolutionResult Resolve(ResolutionResult source)
            {
                return source.New(
                    ((ProfileBase)source.Value)
                    .GetPropertyValue(source.Context.MemberName)
                );
            }
        }
    }
}