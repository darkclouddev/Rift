using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Emotes
{
    public class EmoteExMarkUrl : FormatterBase
    {
        public EmoteExMarkUrl() : base("$emoteExMarkUrl") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, Settings.Emote.ExMarkUrl);
        }
    }
}
