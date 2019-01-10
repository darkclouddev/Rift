using System;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services;
using Rift.Services.Role;

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
                    var role = new TempRole(userId, RoleId, TimeSpan.FromDays(Days));
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role, true);
                }
                else
                {
                    await RiftBot.GetService<RoleService>().AddRoleAsync(userId, RoleId);
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
                RewardString += $" ({Days} дней)";
        }
    }
}
