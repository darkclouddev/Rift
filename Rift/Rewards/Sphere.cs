using System.Threading.Tasks;

namespace Rift.Rewards
{
    public class Sphere
    {
        static readonly Reward PossibleReward =
            new Reward(coins: new Loot(5_000, 10_000),
                       customTickets: new Loot(1, 4),
                       giveawayTickets: new Loot(1, 2),
                       powerupsDoubleExp: new Loot(1, 3),
                       powerupsBotRespect: new Loot(1, 3));

        Reward reward;
        public readonly string RewardString;

        public Sphere(ulong userId)
        {
            RiftBot.Log.Debug($"Opening sphere for {userId.ToString()}");

            reward = PossibleReward.Copy();
            reward.CalculateReward();
            reward.GenerateRewardString();

            RewardString = reward.RewardString;
        }

        public async Task GiveRewardsAsync(ulong userId)
        {
            await reward.GiveRewardAsync(userId);
        }
    }
}
