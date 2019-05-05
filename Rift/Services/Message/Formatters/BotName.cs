using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public class BotName : FormatterBase
    {
        public BotName() : base("$botName") {}

        public override RiftMessage Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, nameof(Rift));
        }
    }
}
