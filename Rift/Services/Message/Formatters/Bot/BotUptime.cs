using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Bot
{
    public class BotUptime : FormatterBase
    {
        public BotUptime() : base("$botUptime") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, GetUptime());
        }
        
        static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize();
    }
}
