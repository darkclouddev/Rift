using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Bot
{
    public class BotUptime : TemplateBase
    {
        public BotUptime() : base(nameof(BotUptime))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, GetUptime());
        }

        static string GetUptime()
        {
            return (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize();
        }
    }
}
