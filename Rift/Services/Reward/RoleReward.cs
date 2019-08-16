using System;
using System.Threading.Tasks;

using Rift.Configuration;

using IonicLib;

using Humanizer;

namespace Rift.Services.Reward
{
    public class RoleReward : RewardBase
    {
        public int RoleId { get; set; }
        public TimeSpan? Duration { get; set; }

        public RoleReward()
        {
            Type = RewardType.Role;
        }

        public RoleReward SetRole(int roleId)
        {
            RoleId = roleId;
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
                if (await DB.RoleInventory.HasAnyAsync(userId, RoleId))
                    return;

                await DB.RoleInventory.AddAsync(userId, RoleId, "Reward");
                return;
            }
            
            var dbRole = await DB.Roles.GetAsync(RoleId);

            if (dbRole is null)
            {
                RiftBot.Log.Error($"No such role ID in database: {RoleId.ToString()}");
                return;
            }
            
            await roleService.AddTempRoleAsync(userId, dbRole.RoleId, Duration.Value, nameof(RoleReward));
        }

        public override string ToString()
        {
            var text = "роль";

            var dbRole = Task.Run(async () => await DB.Roles.GetAsync(RoleId)).Result;

            if (dbRole is null || !IonicClient.GetRole(Settings.App.MainGuildId, dbRole.RoleId, out var role))
            {
                text += " не найдена";
                return text;
            }

            text += $" {role.Name}";

            if (Duration != null)
                text += $" на {Duration.Value.Humanize(culture: RiftBot.Culture)}";

            return text;
        }
    }
}
