using System;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public class BotHeapSize : FormatterBase
    {
        public BotHeapSize() : base("$botHeapSize") {}

        public override RiftMessage Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, GetHeapSize());
        }

        static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}