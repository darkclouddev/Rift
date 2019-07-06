using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

using Microsoft.EntityFrameworkCore.Internal;

namespace Rift.Services.Message.Templates.Economy
{
    public class EconomyTopRichUserList : TemplateBase
    {
        public EconomyTopRichUserList() : base(nameof(EconomyTopRichUserList))
        {
        }

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var emoteService = RiftBot.GetService<EmoteService>();
            var transparentEmote = emoteService.GetEmoteString("$emotetran");
            
            var top = data.Economy.Top10Coins;
            
            var users = top.Select(x => $"{(top.IndexOf(x)+1).ToString()}. <@{x.UserId.ToString()}>{transparentEmote}").ToArray();

            var formattedString = string.Join('\n', users);

            return ReplaceData(message, formattedString);
        }
    }
}