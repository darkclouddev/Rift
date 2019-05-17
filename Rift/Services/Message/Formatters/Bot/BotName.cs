using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Bot
{
    public class BotName : FormatterBase
    {
        public BotName() : base("$botName") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, nameof(Rift));
        }
    }
}
