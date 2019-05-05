using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public class BotVersion : FormatterBase
    {
        public BotVersion() : base("$botVersion") {}

        public override RiftMessage Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, RiftBot.InternalVersion);
        }
    }
}