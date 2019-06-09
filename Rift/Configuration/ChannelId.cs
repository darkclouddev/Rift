using System.Collections.Generic;
using System.Reflection;

using Rift.Attributes;

namespace Rift.Configuration
{
    public class ChannelId
    {
        public Dictionary<string, (string, string)> GetNames()
        {
            var dict = new Dictionary<string, (string, string)>();

            var props = GetType().GetProperties();

            foreach (var prop in props)
            {
                var customAttributes = prop.GetCustomAttributes(true);

                foreach (var attribute in customAttributes)
                {
                    if (attribute is ResolveChannelAttribute resolvedAttribute)
                    {
                        dict.Add(prop.Name, (resolvedAttribute.CategoryName, resolvedAttribute.Name));
                    }
                }
            }

            return dict;
        }

        public void SetValue(string fieldName, ulong value)
        {
            var prop = GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(this, value);
            }
        }

        public ulong Information { get; set; }
        public ulong Comms { get; set; }
        public ulong Clubs { get; set; }
        public ulong Confirmation { get; set; }
        public ulong Live { get; set; }
        public ulong News { get; set; }
        public ulong Modchat { get; set; }
        public ulong Test { get; set; }
        public ulong Logs { get; set; }
        public ulong Search { get; set; }
        public ulong SearchClash { get; set; }
        public ulong Giveaways { get; set; }
        public ulong VoiceSetup { get; set; }
    }
}
