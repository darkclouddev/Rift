using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class SphereReward : ItemReward
    {
        public SphereReward()
        {
            AddTokens(Helper.NextUInt(2, 7));
            AddChests(Helper.NextUInt(3, 7));
            AddTickets(Helper.NextUInt(2, 4));
        }
    }
}
