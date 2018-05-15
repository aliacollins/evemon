using System;

namespace EVEMon.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ParentAttribute : Attribute
    {
        public ParentAttribute(object parent)
        {
            Parent = parent as Enum;
        }

        public Enum Parent { get; }
    }
}
