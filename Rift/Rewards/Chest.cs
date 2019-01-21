using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rift.Rewards
{
    public class Chest : RandomChanceReward
    {
        static readonly List<(uint, Reward)> AvailableRewards = new List<(uint, Reward)>
        {
            (3, new Reward(coins: new Loot(300, 1000), powerupsBotRespect: new Loot(1))),
            (3, new Reward(coins: new Loot(300, 1000), powerupsDoubleExp: new Loot(1))),
            (10, new Reward(coins: new Loot(300, 1000), customTickets: new Loot(1))),
            (100, new Reward(coins: new Loot(300, 1000)))
        };

        public readonly uint Amount;
        public readonly ulong UserId;
        public Reward reward;

        public Chest(ulong userId, uint amount = 1u)
        {
            UserId = userId;
            Amount = amount;

            RiftBot.Log.Debug($"Opening {amount.ToString()} chest(s) for {userId.ToString()}");

            reward = new Reward(coins: 0);
            CalculateLoot(amount);
            reward.CalculateReward();
            reward.GenerateRewardString();
        }

        public async Task GiveRewardAsync()
        {
            await reward.GiveRewardAsync(UserId);
        }

        void CalculateLoot(uint amount)
        {
            for (uint i = 0; i < amount; i++)
                reward = reward + GetReward(AvailableRewards);
        }
    }
}
