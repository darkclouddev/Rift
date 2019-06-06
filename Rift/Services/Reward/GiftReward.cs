using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class GiftReward : ItemReward
    {
        public GiftReward()
        {
            AddRandomCoins(4_000, 10_001);
            AddRandomChests(1, 6);

            if (Helper.GetChance(1u))
                AddDoubleExps(1u);

            if (Helper.GetChance(1u))
                AddTickets(1u);

            if (Helper.GetChance(1u))
                AddBotRespects(1u);
        }
    }
}
