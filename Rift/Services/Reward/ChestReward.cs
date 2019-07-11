using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class ChestReward : ItemReward
    {
        public ChestReward()
        {
            AddRandomCoins(1_600, 2_301);

            if (Helper.GetChance(3u))
                AddTickets(1u);
            else
                AddBotRespects(1u);
        }
    }
}
