using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class ChestReward : ItemReward
    {
        public ChestReward()
        {
            AddCoins(Helper.NextUInt(1600, 2301));

            if (Helper.GetChance(3u))
                AddTickets(1u);
        }
    }
}
