using System;

namespace Infrastructure.EQ.Helpers
{
    public static class EnumHelper
    {
        public static T ToEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}
