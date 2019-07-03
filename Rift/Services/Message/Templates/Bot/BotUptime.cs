using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Bot
{
    public class BotUptime : TemplateBase
    {
        public BotUptime() : base(nameof(BotUptime)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, GetUptime());
        }
        
        static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize();
    }
}
