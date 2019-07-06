using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class EconomyTopActiveUserList : TemplateBase
    {
        public EconomyTopActiveUserList() : base(nameof(EconomyTopActiveUserList))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var emoteService = RiftBot.GetService<EmoteService>();
            var transparentEmote = emoteService.GetEmoteString("$emotetran");
            
            var top = data.Economy.Top10Exp;
            
            var users = top.Select(x => $"{(top.IndexOf(x)+1).ToString()}. <@{x.UserId.ToString()}>{transparentEmote}");

            var formattedString = string.Join('\n', users);

            return ReplaceDataAsync(message, formattedString);
        }
    }
}