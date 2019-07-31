using System;

namespace Rift.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QuestConditionAttribute : Attribute
    {
        public readonly string Description;

        public QuestConditionAttribute(string description)
        {
            Description = description;
        }
    }
}
