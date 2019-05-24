using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Emotes
{
    public class EmoteCoins : FormatterBase
    {
        public EmoteCoins() : base("$emoteCoins") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, Settings.Emote.Coin);
        }
    }
}
