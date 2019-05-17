using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Bot
{
    public class BotVersion : FormatterBase
    {
        public BotVersion() : base("$botVersion") {}

        public override Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, RiftBot.InternalVersion);
        }
    }
}
