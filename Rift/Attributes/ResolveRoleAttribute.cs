using System;

namespace Rift.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveRoleAttribute : Attribute
    {
        string Name { get; }

        public ResolveRoleAttribute(string name)
        {
            Name = name;
        }
    }
}
