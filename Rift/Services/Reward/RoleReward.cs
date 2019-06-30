using System;
using System.Threading.Tasks;

using Rift.Configuration;

using Discord;
using IonicLib;
using Humanizer;

namespace Rift.Services.Reward
{
    public class RoleReward : RewardBase
    {
        public ulong RoleId { get; set; }
        public TimeSpan? Duration { get; set; }

        public RoleReward()
        {
            Type = RewardType.Role;
        }

        public RoleReward SetRole(ulong roleId)
        {
            RoleId = roleId;
            return this;
        }

        public RoleReward SetRole(IRole role)
        {
            RoleId = role.Id;
            return this;
        }

        public RoleReward SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            var roleService = RiftBot.GetService<RoleService>();

            if (Duration is null)
            {
                await roleService.AddPermanentRoleAsync(userId, RoleId);
            }
            else
            {
                await roleService.AddTempRoleAsync(userId, RoleId, Duration.Value, nameof(RoleReward));
            }
        }

        public override string ToString()
        {
            var text = "роль";

            if (!IonicClient.GetRole(Settings.App.MainGuildId, RoleId, out var role))
            {
                text += " не найдена";
                return text;
            }

            text += $" {role.Name}";

            if (Duration != null)
                text += $" на {Duration.Value.Humanize()}";

            return text;
        }
    }
}
