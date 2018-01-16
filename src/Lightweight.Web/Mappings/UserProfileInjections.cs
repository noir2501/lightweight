using Omu.ValueInjecter;
using System.Web.Profile;

namespace Lightweight.Web.Mappings
{
    public class ProfileBaseInjection : KnownTargetValueInjection<ProfileBase>
    {
        protected override void Inject(object source, ref ProfileBase target)
        {
            foreach (var prop in source.GetInfos())
                target.SetPropertyValue(prop.Name, prop.GetValue(source));
        }
    }
}