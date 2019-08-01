using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class ChestReward : ItemReward
    {
        public ChestReward(uint amount)
        {
            for (uint i = 0; i < amount; i++)
            {
                AddRandomCoins(1_500, 2_201);

                if (Helper.GetChance(3u))
                    AddTickets(1u);
                else if (Helper.GetChance(3u))
                    AddDoubleExps(1u);
            }
        }
    }
}
