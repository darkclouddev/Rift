using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Bot
{
    public class BotVersion : FormatterBase
    {
        public BotVersion() : base("$botVersion") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, RiftBot.InternalVersion);
        }
    }
}
