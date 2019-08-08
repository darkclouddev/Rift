using System.Collections.Generic;
using System.Reflection;

using Rift.Attributes;

namespace Rift.Configuration
{
    public class ChannelId
    {
        public Dictionary<string, string> GetNames()
        {
            var dict = new Dictionary<string, string>();

            var props = GetType().GetProperties();

            foreach (var prop in props)
            {
                var customAttributes = prop.GetCustomAttributes(true);

                foreach (var attribute in customAttributes)
                    if (attribute is ResolveChannelAttribute resolvedAttribute)
                        dict.Add(prop.Name, resolvedAttribute.Name);
            }

            return dict;
        }

        public void SetValue(string fieldName, ulong value)
        {
            var prop = GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
                prop.SetValue(this, value);
        }

        [ResolveChannel("общение")]
        public ulong Chat { get; set; }

        [ResolveChannel("команды")]
        public ulong Commands { get; set; }

        [ResolveChannel("логи")]
        public ulong Logs { get; set; }

        [ResolveChannel("Создать канал")]
        public ulong VoiceSetup { get; set; }

        [ResolveChannel("монстры")]
        public ulong Monsters { get; set; }
        
        public ulong Afk { get; set; }
    }
}
