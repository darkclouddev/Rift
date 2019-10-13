using System.Threading.Tasks;

using IonicLib;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class TicketGiveawayWinnerUserName : TemplateBase
    {
        public TicketGiveawayWinnerUserName() : base(nameof(TicketGiveawayWinnerUserName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, data.Giveaway.TicketGiveaway.WinnerId, out var sgUser))
            {
                TemplateError("No user data found.");
                return null;
            }

            return ReplaceDataAsync(message, sgUser.Username);
        }
    }
}
