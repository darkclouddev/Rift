using System;

namespace Rift.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveChannelAttribute : Attribute
    {
        public string CategoryName { get; set; }
        public string Name { get; set; }

        public ResolveChannelAttribute(string categoryName, string name)
        {
            CategoryName = categoryName;
            Name = name;
        }
    }
}
