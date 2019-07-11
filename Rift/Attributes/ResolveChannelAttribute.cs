using System;

namespace Rift.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveChannelAttribute : Attribute
    {
        public string CategoryName { get; }
        public string Name { get; }

        public ResolveChannelAttribute(string categoryName, string name)
        {
            CategoryName = categoryName;
            Name = name;
        }
    }
}
