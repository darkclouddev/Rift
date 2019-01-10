using System.Collections.Generic;

using IonicLib.Extensions;

namespace Rift.Rewards
{
    public class EventReward
    {
        public static Reward RandomReward => AvailableRewards.Random();

        public static Reward DrakeWinner = new Reward(chests: 4, giveawayTickets: 1);

        public static Reward DrakeGeneral = new Reward(coins: 3000, chests: 1);

        public static Reward BaronGeneral = new Reward(coins: 3000, chests: 1);

        public static Reward BaronWinner = new Reward(chests: 6, powerupsBotRespect: 1);

        static readonly List<Reward> AvailableRewards = new List<Reward>
        {
            new Reward(coins: 500, chests: 1),
            new Reward(coins: 1000, chests: 1),
            new Reward(coins: 1500, chests: 1),
        };
    }
}
