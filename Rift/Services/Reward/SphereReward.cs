using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class SphereReward : ItemReward
    {
        public SphereReward()
        {
            AddRandomTokens(2, 7);
            AddRandomChests(3, 7);
            AddRandomTickets(2, 4);

            if (Helper.GetChance(45u))
                AddBotRespects(1u);

            if (Helper.GetChance(45u))
                AddDoubleExps(1u);
        }
    }
}
