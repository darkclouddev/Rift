using System;

namespace Rift.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveChannelAttribute : Attribute
    {
        public string Name { get; }

        public ResolveChannelAttribute(string name)
        {
            Name = name;
        }
    }
}
