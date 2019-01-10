using System.Collections.Generic;

using IonicLib.Extensions;

namespace Rift.Rewards
{
    public class RandomChanceReward
    {
        protected Reward GetReward(List<(uint, Reward)> AvailableRewards)
        {
            var curChance = Helper.NextUInt(0, 101);
            uint chance = 0;

            foreach (var (item1, reward) in AvailableRewards)
            {
                chance += item1;

                if (curChance <= chance)
                    return reward;
            }

            return null;
        }
    }
}
