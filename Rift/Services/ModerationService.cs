using System;
using System.Linq;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;

using Discord;
using Discord.WebSocket;
using IonicLib;

namespace Rift.Services
{
    public class ModerationService
    {
        public async Task KickAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            await sgTarget.KickAsync();

            var data = new FormatData
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason,
                }
            };

            await RiftBot.SendChatMessageAsync("mod-kick-success", data);
        }

        public async Task BanAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            if (RiftBot.IsAdmin(sgTarget) || RiftBot.IsModerator(sgTarget))
            {
                await RiftBot.SendChatMessageAsync("mod-friendly-fire", new FormatData(moderator.Id));
                return;
            }

            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            await guild.AddBanAsync(sgTarget, 1, $"Banned by {moderator}: {reason}");

            var data = new FormatData
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason,
                }
            };

            await RiftBot.SendChatMessageAsync("mod-ban-success", data);
        }

        public async Task MuteAsync(IUser target, string reason, string time, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            if (RiftBot.IsAdmin(sgTarget) || RiftBot.IsModerator(sgTarget))
            {
                await RiftBot.SendChatMessageAsync("mod-friendly-fire", new FormatData(moderator.Id));
                return;
            }

            if (!int.TryParse(time.Remove(time.Length - 1), out var timeInt))
            {
                await RiftBot.SendChatMessageAsync("mod-wrong-time-format", new FormatData(moderator.Id));
                return;
            }

            TimeSpan ts;

            var timeMod = time.Last();

            switch (timeMod)
            {
                case 's':
                    ts = TimeSpan.FromSeconds(timeInt);
                    break;

                case 'm':
                    ts = TimeSpan.FromMinutes(timeInt);
                    break;

                case 'h':
                    ts = TimeSpan.FromHours(timeInt);
                    break;

                case 'd':
                    ts = TimeSpan.FromDays(timeInt);
                    break;

                default:
                    await RiftBot.SendChatMessageAsync("mod-wrong-time-format", new FormatData(moderator.Id));
                    return;
            }

            await RiftBot.GetService<RoleService>().AddTempRoleAsync(sgTarget.Id, Settings.RoleId.Muted, ts,
                    $"Muted by {moderator}|{moderator.Id.ToString()} with reason: {reason}");

            var data = new FormatData
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason,
                    Duration = ts
                }
            };

            await RiftBot.SendChatMessageAsync("mod-mute-success", data);
        }

        public async Task UnmuteAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            await RiftBot.GetService<RoleService>().RemoveTempRoleAsync(sgTarget.Id, Settings.RoleId.Muted);
        }

        public async Task WarnAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            var data = new FormatData
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason
                }
            };

            await RiftBot.SendChatMessageAsync("mod-warn-success", data);
        }

        static async Task<(bool, SocketGuildUser)> ValidateAsync(IUser target, string reason, IUser moderator)
        {
            if (string.IsNullOrEmpty(reason))
            {
                await RiftBot.SendChatMessageAsync("mod-empty-reason", new FormatData(moderator.Id));
                return (false, null);
            }

            if (!(target is SocketGuildUser sgTarget))
            {
                await RiftBot.SendChatMessageAsync("mod-user-not-found", new FormatData(moderator.Id));
                return (false, null);
            }

            return (true, sgTarget);
        }
    }
}
