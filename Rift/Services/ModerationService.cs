using System;
using System.Linq;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;

using Discord;
using Discord.WebSocket;
using Humanizer;
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
            await Database.AddModerationLogAsync(sgTarget.Id, moderator.Id, "Kick", reason, DateTime.UtcNow, TimeSpan.Zero);

            var data = new FormatData(target.Id)
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
            await Database.AddModerationLogAsync(sgTarget.Id, moderator.Id, "Ban", reason, DateTime.UtcNow, TimeSpan.Zero);

            var data = new FormatData(target.Id)
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

            await Database.AddModerationLogAsync(sgTarget.Id, moderator.Id, "Mute", reason, DateTime.UtcNow, ts);

            var data = new FormatData(sgTarget.Id)
            {
                Moderation = new ModerationData
                {
                    TargetId = sgTarget.Id,
                    ModeratorId = moderator.Id,
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
            await Database.AddModerationLogAsync(sgTarget.Id, moderator.Id, "Unmute", reason, DateTime.UtcNow, TimeSpan.Zero);
        }

        public async Task WarnAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            await Database.AddModerationLogAsync(sgTarget.Id, moderator.Id, "Warn", reason, DateTime.UtcNow, TimeSpan.Zero);

            (var oldToxicity, var newToxicity) = await GetNewToxicityAsync(sgTarget.Id, ToxicitySource.Warn);

            await Database.AddOrUpdateToxicityAsync(sgTarget.Id, newToxicity.Percent);

            if (newToxicity.Level > oldToxicity.Level)
                await RiftBot.SendChatMessageAsync("mod-toxicity-increased", new FormatData(sgTarget.Id));

            var data = new FormatData(sgTarget.Id)
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

        static async Task<(RiftToxicity, RiftToxicity)> GetNewToxicityAsync(ulong userId, ToxicitySource source)
        {
            var currentToxicity = await Database.GetToxicityAsync(userId) ?? new RiftToxicity
            {
                UserId = userId,
                Percent = 0u,
            };

            var newToxicity = new RiftToxicity
            {
                UserId = userId,
            };

            uint addend;

            switch (source)
            {
                case ToxicitySource.Warn:
                    addend = 1u;
                    break;

                case ToxicitySource.Mute:
                    addend = 5u;
                    break;

                case ToxicitySource.Kick:
                    addend = 20u;
                    break;

                case ToxicitySource.Ban:
                    addend = 50u;
                    break;

                default:
                    addend = 0u;
                    break;
            }

            newToxicity.Percent = Math.Min(currentToxicity.Percent + addend, 100u);

            return (currentToxicity, newToxicity);
        }

        public async Task<IonicMessage> GetUserActionLogsAsync(IUser user)
        {
            if (user is null)
                return MessageService.UserNotFound;

            var list = await Database.GetModerationLogsForUserAsync(user.Id);
            var toxicity = await Database.GetToxicityAsync(user.Id);

            var actions = string.Join('\n', list.Select(x =>
            {
                var action = FormatAction(x.Action);

                if (x.Duration != TimeSpan.Zero)
                    action += $"({x.Duration.Humanize()})";

                return action;
            }));

            var datetime = string.Join('\n', list.Select(x =>
                x.CreatedAt.Humanize()));

            var moderator = string.Join('\n', list.Select(x =>
                IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.ModeratorId).Username));

            var embed = new RiftEmbed
            {
                Description = $"Досье товарища {user.Username}\nУровень токсичности: {FormatToxicityLevel(toxicity.Level)}",
                Fields = new []
                {
                    new RiftField("Действие", actions, true),
                    new RiftField("Дата и время", datetime, true),
                    new RiftField("Модератор", moderator, true),
                }
            };

            return new IonicMessage(embed);
        }

        public async Task<IonicMessage> GetLastActionsAsync()
        {
            var list = await Database.GetLastModerationLogsAsync();

            var mods = string.Join('\n', list.Select(x =>
                IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.ModeratorId).Username));

            var targets = string.Join('\n', list.Select(x =>
                IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.TargetId).Username));

            var actions = string.Join('\n', list.Select(x =>
            {
                var action = FormatAction(x.Action);

                if (x.Duration != TimeSpan.Zero)
                    action += $"({x.Duration.Humanize()})";

                return action;
            }));

            var embed = new RiftEmbed
            {
                Description = "Последние действия банхаммером",
                Fields = new[]
                {
                    new RiftField("Модератор", mods, true),
                    new RiftField("Нарушитель", targets, true),
                    new RiftField("Действие", actions, true),
                }
            };

            return new IonicMessage(embed);
        }

        static string FormatAction(string action)
        {
            switch (action.ToLowerInvariant())
            {
                case "kick": return "Выгнан с сервера";
                case "ban": return "Бан";
                case "warn": return "Предупреждение";
                case "mute": return "Мут ";
                case "unmute": return "Снят мут";

                default: return string.Empty;
            }
        }

        static string FormatToxicityLevel(uint level)
        {
            return RiftBot.GetService<EmoteService>().GetEmoteString("$emotet" + level.ToString());
        }

        enum ToxicitySource
        {
            Warn,
            Mute,
            Kick,
            Ban,
        }
    }
}
