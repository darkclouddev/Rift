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
            var emoteService = RiftBot.GetService<EmoteService>();
            var lvlEmote = emoteService.GetEmoteString("$emotelvl");
            var expEmote = emoteService.GetEmoteString("$emoteexp");
            
            var top = data.Economy.Top10Exp;
            
            var coins = top.Select(x => $"{lvlEmote} {x.Level.ToString()} {expEmote} {x.Experience.ToString()}");

            var formattedString = string.Join('\n', coins);

            return ReplaceDataAsync(message, formattedString);
        }
    }
}