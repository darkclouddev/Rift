using System;
using System.Text;
using System.Threading.Tasks;

using Humanizer;

using IonicLib;

using Rift.Services.Interfaces;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class RewardService : IRewardService
    {
        readonly IEmoteService emoteService;
        readonly IRoleService roleService;

        public RewardService(IEmoteService emoteService,
                             IRoleService roleService)
        {
            this.emoteService = emoteService;
            this.roleService = roleService;
        }

        public async Task DeliverToAsync(ulong userId, RewardBase reward)
        {
            if (reward is null)
            {
                RiftBot.Log.Warning($"Trying to give an empty reward to {userId.ToString()}");
                return;
            }
            
            switch (reward.Type)
            {
                case RewardType.Item:
                case RewardType.Capsule:
                case RewardType.Chest:
                case RewardType.Gift:
                case RewardType.Sphere:
                    await DeliverItemAsync(userId, reward as ItemReward);
                    break;
                
                case RewardType.Role:
                    await DeliverRoleAsync(userId, reward as RoleReward);
                    break;
                
                case RewardType.Background:
                    await DeliverBackgroundAsync(userId, reward as BackgroundReward);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        async Task DeliverItemAsync(ulong userId, ItemReward reward)
        {
            await DB.Inventory.AddAsync(userId, reward.ToInventoryData());
        }

        async Task DeliverRoleAsync(ulong userId, RoleReward reward)
        {
            if (reward.Duration is null)
            {
                if (await DB.RoleInventory.HasAnyAsync(userId, reward.RoleId))
                    return;

                await DB.RoleInventory.AddAsync(userId, reward.RoleId, "Reward");
                return;
            }
            
            var dbRole = await DB.Roles.GetAsync(reward.RoleId);
            if (dbRole is null)
            {
                RiftBot.Log.Error($"No such role ID in database: {reward.RoleId.ToString()}");
                return;
            }
            
            await roleService.AddTempRoleAsync(userId, dbRole.RoleId, reward.Duration.Value, nameof(RoleReward));
        }
        
        async Task DeliverBackgroundAsync(ulong userId, BackgroundReward reward)
        {
            await DB.BackgroundInventory.AddAsync(userId, reward.BackgroundId);
        }

        public string Format(ItemReward reward)
        {
            var sb = new StringBuilder();

            if (reward.Coins.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emotecoins")} {reward.Coins.Value.ToString()} ");
            if (reward.Tokens.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emotetokens")} {reward.Tokens.Value.ToString()} ");
            if (reward.Chests.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emotechest")} {reward.Chests.Value.ToString()} ");
            if (reward.Spheres.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emotesphere")} {reward.Spheres.Value.ToString()} ");
            if (reward.Capsules.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emotecapsule")} {reward.Capsules.Value.ToString()} ");
            if (reward.Tickets.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emoteticket")} {reward.Tickets.Value.ToString()} ");
            if (reward.DoubleExps.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emote2exp")} {reward.DoubleExps.Value.ToString()} ");
            if (reward.BotRespects.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emoterespect")} {reward.BotRespects.Value.ToString()} ");
            if (reward.Rewinds.HasValue)
                sb.Append($"{emoteService.GetEmoteString("$emoterewind")} {reward.Rewinds.Value.ToString()} ");

            return sb.ToString().TrimEnd();
        }
        
        public string ToPlainString(ItemReward reward)
        {
            var sb = new StringBuilder();

            if (reward.Coins.HasValue)
                sb.Append($"{nameof(reward.Coins)} {reward.Coins.Value.ToString()} ");
            if (reward.Tokens.HasValue)
                sb.Append($"{nameof(reward.Tokens)} {reward.Tokens.Value.ToString()} ");
            if (reward.Chests.HasValue)
                sb.Append($"{nameof(reward.Chests)} {reward.Chests.Value.ToString()} ");
            if (reward.Spheres.HasValue)
                sb.Append($"{nameof(reward.Spheres)} {reward.Spheres.Value.ToString()} ");
            if (reward.Capsules.HasValue)
                sb.Append($"{nameof(reward.Capsules)} {reward.Capsules.Value.ToString()} ");
            if (reward.Tickets.HasValue)
                sb.Append($"{nameof(reward.Tickets)} {reward.Tickets.Value.ToString()} ");
            if (reward.DoubleExps.HasValue)
                sb.Append($"{nameof(reward.DoubleExps)} {reward.DoubleExps.Value.ToString()} ");
            if (reward.BotRespects.HasValue)
                sb.Append($"{nameof(reward.BotRespects)} {reward.BotRespects.Value.ToString()} ");
            if (reward.Rewinds.HasValue)
                sb.Append($"{nameof(reward.Rewinds)} {reward.Rewinds.Value.ToString()} ");

            return sb.ToString().TrimEnd();
        }

        public async Task<string> FormatAsync(RoleReward reward)
        {
            var text = "роль";

            var dbRole = await DB.Roles.GetAsync(reward.RoleId);
            if (dbRole is null || !IonicHelper.GetRole(Settings.App.MainGuildId, dbRole.RoleId, out var role))
            {
                text += " не найдена";
                return text;
            }

            text += $" {role.Name}";

            if (reward.Duration != null)
                text += $" на {reward.Duration.Value.Humanize(culture: RiftBot.Culture)}";

            return text;
        }

        public async Task<string> FormatAsync(BackgroundReward reward)
        {
            var text = "фон ";
            var dbBack = await DB.ProfileBackgrounds.GetAsync(reward.BackgroundId);
            if (dbBack is null)
            {
                RiftBot.Log.Error($"Background ID \"{nameof(reward.BackgroundId)}\" does not exist!");
                text += "не найден";
                return text;
            }

            text += dbBack.Name;
            return text;
        }
    }
}
