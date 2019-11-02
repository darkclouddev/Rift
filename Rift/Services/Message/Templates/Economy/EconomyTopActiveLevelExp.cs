using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class EconomyTopActiveLevelExp : TemplateBase
    {
        public EconomyTopActiveLevelExp() : base(nameof(EconomyTopActiveLevelExp))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var lvlEmote = data.EmoteService.GetEmoteString("$emotelvl");
            var expEmote = data.EmoteService.GetEmoteString("$emoteexp");

            var top = data.Economy.Top10Exp;

            var coins = top.Select(x => $"{lvlEmote} {x.Level.ToString()} {expEmote} {x.Experience.ToString()}");

            var formattedString = string.Join('\n', coins);

            return ReplaceDataAsync(message, formattedString);
        }
    }
}
