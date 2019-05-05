using System;
using System.Diagnostics;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters
{
    public class BotUptime : FormatterBase
    {
        public BotUptime() : base("$botUptime") {}

        public override RiftMessage Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, GetUptime());
        }
        
        static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(5);
    }
}