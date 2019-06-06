using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Guild.Channels
{
    public class ChannelRegistration : FormatterBase
    {
        public ChannelRegistration() : base("$channelRegistration") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<#{Settings.ChannelId.Confirmation.ToString()}>");
        }
    }
}
