using System.Collections.Generic;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Rewards;
using Rift.Services.Giveaway;

namespace Rift.Services
{
    public class GiveawayService
    {
        static List<(Reward, TicketType)> giveaways = new List<(Reward, TicketType)>
        {
            (new Reward(chests: 6u), TicketType.NoTicket),
            (new Reward(chests: 10u, powerupsBotRespect: 2u), TicketType.NoTicket),
            (new Reward(chests: 10u, powerupsDoubleExp: 2u), TicketType.NoTicket),
            (new RoleReward(Settings.RoleId.Legendary, 30u), TicketType.NoTicket),
            (new RoleReward(Settings.RoleId.YasuoPlayer, 30u), TicketType.NoTicket),
            (new Reward(coins: 6_000u), TicketType.Usual),
            (new Reward(coins: 12_000u), TicketType.Usual),
            (new Reward(spheres: 1u), TicketType.Usual),
            (new Reward(capsules: 1u), TicketType.Usual),
            (new Reward(coins: 40_000u), TicketType.Rare),
        };

        public GiveawayService()
        {
            foreach (var giveaway in giveaways)
            {
                giveaway.Item1.CalculateReward();
                giveaway.Item1.GenerateRewardString();
            }
        }

        public async Task GiveawayStartAsync(int number)
        {
            --number;
            if (number < 0 || number > giveaways.Count)
                return;

            var (reward, type) = giveaways[number];

            if (type == TicketType.NoTicket)
                await new FreeGiveaway(reward).StartGiveawayAsync();
            else
                await new TicketGiveaway(reward, type).StartGiveawayAsync();
        }
    }
}
