using System;
using Lightweight.Business.Exceptions;

namespace Lightweight.Business.Helpers
{
    public sealed class EnumHelper<TEnum> where TEnum : struct
    {
        public static TEnum GetEnumValue(string name)
        {
            TEnum result;
            if (!Enum.TryParse<TEnum>(name, out result))
                throw new BusinessException(string.Format("Cannot convert value '{0}' to type {1}", name, typeof(TEnum).Name));

            return result;
        }
    }
}
