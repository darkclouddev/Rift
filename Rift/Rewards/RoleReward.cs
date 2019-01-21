using System;
using System.Threading.Tasks;

using Humanizer;

using Rift.Configuration;
using Rift.Services;

namespace Rift.Rewards
{
    public class RoleReward : Reward
    {
        public ulong RoleId = 0;
        public ulong Days = 0;

        public RoleReward(ulong roleId, ulong Days = 0, Loot coins = null, Loot tokens = null, Loot chests = null,
                          Loot spheres = null, Loot capsules = null, Loot powerupsAttack = null,
                          Loot powerupsDoubleExp = null, Loot powerupsBotRespect = null, Loot customTickets = null,
                          Loot giveawayTickets = null, Loot experience = null)
            : base(coins, tokens, chests, spheres, capsules,
                   powerupsDoubleExp, powerupsBotRespect,
                   customTickets, giveawayTickets, experience)
        {
            RoleId = roleId;
        }

        public override async Task GiveRewardAsync(ulong userId)
        {
            await base.GiveRewardAsync(userId);

            if (RoleId != 0)
            {
                if (Days != 0)
                {
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(userId, RoleId, TimeSpan.FromDays(Days), nameof(RoleReward).Humanize());
                }
                else
                {
                    await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, RoleId);
                }
            }
        }

        public override void GenerateRewardString()
        {
            if (RoleId == Settings.RoleId.Legendary)
                RewardString = $"роль {Settings.Emote.Legendary} Легендарные";
            else
                RewardString = $"роль {Settings.Emote.Roles} {EconomyService.TempRoles[RoleId]}";

            if (Days > 0)
                RewardString += $" ({Days.ToString()} дней)";
        }
    }
}
