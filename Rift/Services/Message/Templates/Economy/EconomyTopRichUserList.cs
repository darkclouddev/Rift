using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class EconomyTopRichUserList : TemplateBase
    {
        public EconomyTopRichUserList() : base(nameof(EconomyTopRichUserList))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var emoteService = RiftBot.GetService<EmoteService>();
            var transparentEmote = emoteService.GetEmoteString("$emotetran");

            var top = data.Economy.Top10Coins;

            var users = top.Select(
                x => $"{(top.IndexOf(x) + 1).ToString()}. <@{x.UserId.ToString()}>{transparentEmote}");

            var formattedString = string.Join('\n', users);

            return ReplaceDataAsync(message, formattedString);
        }
    }
}
