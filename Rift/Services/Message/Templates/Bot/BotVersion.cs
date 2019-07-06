using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Bot
{
    public class BotVersion : TemplateBase
    {
        public BotVersion() : base(nameof(BotVersion)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, RiftBot.InternalVersion);
        }
    }
}
