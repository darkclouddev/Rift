using System;
using System.Globalization;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Bot
{
    public class BotHeapSize : TemplateBase
    {
        public BotHeapSize() : base(nameof(BotHeapSize)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, GetHeapSize());
        }

        static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)
                .ToString(new CultureInfo("en-US"));
    }
}
