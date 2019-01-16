using System.Threading.Tasks;
using System.Collections.Generic;

using Rift.Configuration;
using Rift.Services;

namespace Rift.Rewards
{
    public class Capsule : RandomRoleReward
    {
        static List<ulong> AvailableRoles = new List<ulong>()
        {
            Settings.RoleId.Epic,
            Settings.RoleId.Hasagi,
            Settings.RoleId.Ward,
            Settings.RoleId.Reworked,
            Settings.RoleId.Meta
        };

        static Reward PossibleReward =
            new Reward(coins: new Loot(70_000, 140_000),
                       tokens: new Loot(8, 16),
                       powerupsDoubleExp: new Loot(2, 6),
                       customTickets: new Loot(4, 10),
                       giveawayTickets: new Loot(4, 10));

        public Reward reward;
        public readonly string RewardString;
        public static uint days = 60;

        public Capsule(ulong userId)
        {
            RiftBot.Log.Debug($"Opening capsule for {userId.ToString()}");

            reward = PossibleReward.Copy();
            reward.CalculateReward();
            reward.GenerateRewardString();

            Task.Run(async () => await GetRole(AvailableRoles, userId));

            RewardString = reward.RewardString;

            if (RoleId != 0)
                RoleString = $"{Settings.Emote.Roles} {EconomyService.TempRoles[RoleId]}";
        }

        public async Task GiveRewardsAsync(ulong userId)
        {
            await reward.GiveRewardAsync(userId);
            await GiveRoleAsync(userId, days);
        }
    }
}
