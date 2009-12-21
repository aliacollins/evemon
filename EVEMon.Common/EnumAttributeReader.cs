using System;
using System.Reflection;

namespace EVEMon.Common
{
    public static class EnumAttributeReader<TEnum, TAttribute>
        where TAttribute : Attribute
    {
        public static TAttribute GetAttribute(TEnum value)
        {
            // Unfortunately this can't be done at compile time...
            if (!typeof (TEnum).IsSubclassOf(typeof (Enum)))
            {
                throw new InvalidOperationException("TEnum must be a subclass of System.Enum");
            }

            MemberInfo[] memInfo = typeof (TEnum).GetMember(value.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof (TAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return (TAttribute) attrs[0];
                }
            }
            return null;
        }
    }
}