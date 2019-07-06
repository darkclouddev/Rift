using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class EconomyTopRichCoins : TemplateBase
    {
        public EconomyTopRichCoins() : base(nameof(EconomyTopRichCoins))
        {
        }

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var emoteService = RiftBot.GetService<EmoteService>();
            var coinsEmote = emoteService.GetEmoteString("$emotecoins");
            
            var top = data.Economy.Top10Coins;
            
            var coins = top.Select(x => $"{coinsEmote} {x.Coins.ToString()}").ToArray();

            var formattedString = string.Join('\n', coins);

            return ReplaceData(message, formattedString);
        }
    }
}