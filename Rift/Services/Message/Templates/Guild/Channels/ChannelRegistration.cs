using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Guild.Channels
{
    public class ChannelRegistration : TemplateBase
    {
        public ChannelRegistration() : base(nameof(ChannelRegistration)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<#{Settings.ChannelId.Confirmation.ToString()}>");
        }
    }
}
