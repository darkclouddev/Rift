using System;
using System.Globalization;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Bot
{
    public class BotHeapSize : FormatterBase
    {
        public BotHeapSize() : base("$botHeapSize") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, GetHeapSize());
        }

        static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)
                .ToString(new CultureInfo("en-US"));
    }
}
