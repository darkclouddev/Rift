using System.Collections.Generic;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Rewards;

using IonicLib;
using IonicLib.Util;

namespace Rift.Services.Giveaway
{
    class TicketGiveaway : GiveawayBase
    {
        public TicketType Ticket;
        public string ticketString;

        public TicketGiveaway(Reward reward, TicketType ticket)
            : base(reward)
        {
            Ticket = ticket;
            if (ticket == TicketType.Usual)
                ticketString = $"{Settings.Emote.UsualTickets} обычный билет";
            else
                ticketString = $"{Settings.Emote.RareTickets} редкий билет";
        }

        public override async Task StartGiveawayAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Giveaways, out var channel))
                return;

            bool isUsualTicket = Ticket == TicketType.Usual;

            var userList = isUsualTicket
                ? await RiftBot.GetService<DatabaseService>().GetUsersWithUsualTicketsAsync()
                : await RiftBot.GetService<DatabaseService>().GetUsersWithRareTicketsAsync();

            users = new List<ulong>();

            foreach (var user in userList)
            {
                if (isUsualTicket && user.UsualTickets > 0)
                {
                    await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(user.UserId, usualTickets: 1);
                    users.Add(user.UserId);
                }
                else if (!isUsualTicket && user.RareTickets > 0)
                {
                    await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(user.UserId, rareTickets: 1);
                    users.Add(user.UserId);
                }
            }

            await channel.SendEmbedAsync(GiveawayEmbeds.ChatTicket(reward, ticketString, users.Count));

            await FinishGiveaway();
        }
    }
}
