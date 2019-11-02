using System;
using System.Linq;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Interfaces;
using Rift.Services.Message;

using Discord;
using Discord.WebSocket;

using Humanizer;

using IonicLib;

namespace Rift.Services
{
    public class ModerationService : IModerationService
    {
        readonly IMessageService messageService;
        readonly IEmoteService emoteService;
        readonly IRoleService roleService;
        
        public ModerationService(IMessageService messageService,
                                 IEmoteService emoteService,
                                 IRoleService roleService)
        {
            this.messageService = messageService;
            this.emoteService = emoteService;
            this.roleService = roleService;
        }
        
        public async Task KickAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            await sgTarget.KickAsync();
            await DB.ModerationLogs.AddAsync(sgTarget.Id, moderator.Id, "Kick", reason, DateTime.UtcNow, TimeSpan.Zero);

            (var oldToxicity, var newToxicity) = await GetNewToxicityAsync(sgTarget.Id, ToxicitySource.Kick);
            await DB.Toxicity.UpdatePercentAsync(sgTarget.Id, newToxicity.Percent);

            var data = new FormatData(target.Id)
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason,
                }
            };

            await messageService.SendMessageAsync("mod-kick-success", Settings.ChannelId.Chat, data);
        }

        public async Task BanAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            if (RiftBot.IsAdmin(sgTarget) || await RiftBot.IsModeratorAsync(sgTarget))
            {
                await messageService.SendMessageAsync("mod-friendly-fire", Settings.ChannelId.Commands, new FormatData(moderator.Id));
                return;
            }

            if (!IonicHelper.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            await DB.ModerationLogs.AddAsync(sgTarget.Id, moderator.Id, "Ban", reason, DateTime.UtcNow, TimeSpan.Zero);

            (var oldToxicity, var newToxicity) = await GetNewToxicityAsync(sgTarget.Id, ToxicitySource.Ban);
            await DB.Toxicity.UpdatePercentAsync(sgTarget.Id, newToxicity.Percent);

            var data = new FormatData(target.Id)
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason,
                }
            };

            await messageService.SendMessageAsync("mod-ban-success", Settings.ChannelId.Chat, data);
            await guild.AddBanAsync(sgTarget, 1, $"Banned by {moderator}: {reason}");
        }

        public async Task MuteAsync(IUser target, string reason, string time, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            if (RiftBot.IsAdmin(sgTarget) || await RiftBot.IsModeratorAsync(sgTarget))
            {
                await messageService.SendMessageAsync("mod-friendly-fire", Settings.ChannelId.Commands, new FormatData(moderator.Id));
                return;
            }

            if (!int.TryParse(time.Remove(time.Length - 1), out var timeInt))
            {
                await messageService.SendMessageAsync("mod-wrong-time-format", Settings.ChannelId.Commands, new FormatData(moderator.Id));
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
                    await messageService.SendMessageAsync("mod-wrong-time-format", Settings.ChannelId.Commands, new FormatData(moderator.Id));
                    return;
            }

            var muted = await DB.Roles.GetAsync(40);
            await roleService.AddTempRoleAsync(sgTarget.Id, muted.RoleId, ts,
                                                                     $"Muted by {moderator}|{moderator.Id.ToString()} with reason: {reason}");
            await DB.ModerationLogs.AddAsync(sgTarget.Id, moderator.Id, "Mute", reason, DateTime.UtcNow, ts);

            (var oldToxicity, var newToxicity) = await GetNewToxicityAsync(sgTarget.Id, ToxicitySource.Mute);
            await DB.Toxicity.UpdatePercentAsync(sgTarget.Id, newToxicity.Percent);

            if (newToxicity.Level > oldToxicity.Level)
                await messageService.SendMessageAsync("mod-toxicity-increased", Settings.ChannelId.Chat, new FormatData(sgTarget.Id));

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

            await messageService.SendMessageAsync("mod-mute-success", Settings.ChannelId.Chat, data);
        }

        public async Task UnmuteAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            var muted = await DB.Roles.GetAsync(40);
            await roleService.RemoveTempRoleAsync(sgTarget.Id, muted.RoleId);
            await DB.ModerationLogs.AddAsync(sgTarget.Id, moderator.Id, "Unmute", reason, DateTime.UtcNow,
                                             TimeSpan.Zero);
        }

        public async Task WarnAsync(IUser target, string reason, IUser moderator)
        {
            (var passed, var sgTarget) = await ValidateAsync(target, reason, moderator);

            if (!passed)
                return;

            await DB.ModerationLogs.AddAsync(sgTarget.Id, moderator.Id, "Warn", reason, DateTime.UtcNow, TimeSpan.Zero);

            (var oldToxicity, var newToxicity) = await GetNewToxicityAsync(sgTarget.Id, ToxicitySource.Warn);
            await DB.Toxicity.UpdatePercentAsync(sgTarget.Id, newToxicity.Percent);

            if (newToxicity.Level > oldToxicity.Level)
                await messageService.SendMessageAsync("mod-toxicity-increased", Settings.ChannelId.Chat, new FormatData(sgTarget.Id));

            var data = new FormatData(sgTarget.Id)
            {
                Moderation = new ModerationData
                {
                    ModeratorId = moderator.Id,
                    TargetId = sgTarget.Id,
                    Reason = reason
                }
            };

            await messageService.SendMessageAsync("mod-warn-success", Settings.ChannelId.Chat, data);
        }

        async Task<(bool, SocketGuildUser)> ValidateAsync(IUser target, string reason, IUser moderator)
        {
            if (string.IsNullOrEmpty(reason))
            {
                await messageService.SendMessageAsync("mod-empty-reason", Settings.ChannelId.Commands, new FormatData(moderator.Id));
                return (false, null);
            }

            if (!(target is SocketGuildUser sgTarget))
            {
                await messageService.SendMessageAsync("mod-user-not-found", Settings.ChannelId.Commands, new FormatData(moderator.Id));
                return (false, null);
            }

            return (true, sgTarget);
        }

        static async Task<(RiftToxicity, RiftToxicity)> GetNewToxicityAsync(ulong userId, ToxicitySource source)
        {
            var currentToxicity = await DB.Toxicity.GetAsync(userId) ?? new RiftToxicity
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

            var list = await DB.ModerationLogs.GetAsync(user.Id);
            var toxicity = await DB.Toxicity.GetAsync(user.Id);

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
            {
                IonicHelper.GetGuildUserById(Settings.App.MainGuildId, x.ModeratorId, out var sgUser);
                return sgUser is null ? "-" : sgUser.Username;
            }));

            var embed = new RiftEmbed()
                .WithDescription($"Досье товарища {user.Username}\nУровень токсичности: {FormatToxicityLevel(toxicity.Level)}")
                .AddField("Действие", actions, true)
                .AddField("Дата и время", datetime, true)
                .AddField("Модератор", moderator, true);

            return new IonicMessage(embed);
        }

        public async Task<IonicMessage> GetLastActionsAsync()
        {
            var list = await DB.ModerationLogs.GetLastTenAsync();

            var mods = string.Join('\n', list.Select(x =>
            {
                IonicHelper.GetGuildUserById(Settings.App.MainGuildId, x.ModeratorId, out var sgUser);
                return sgUser is null ? "-" : sgUser.Username;
            }));

            var targets = string.Join('\n', list.Select(x =>
            {
                IonicHelper.GetGuildUserById(Settings.App.MainGuildId, x.TargetId, out var sgUser);
                return sgUser is null ? "-" : sgUser.Username;
            }));

            var actions = string.Join('\n', list.Select(x =>
            {
                var action = FormatAction(x.Action);

                if (x.Duration != TimeSpan.Zero)
                    action += $"({x.Duration.Humanize()})";

                return action;
            }));

            var embed = new RiftEmbed()
                .WithDescription("Последние действия банхаммером")
                .AddField("Модератор", mods, true)
                .AddField("Нарушитель", targets, true)
                .AddField("Действие", actions, true);

            return new IonicMessage(embed);
        }

        static string FormatAction(string action)
        {
            return action.ToLowerInvariant() switch
            {
                "kick" => "Выгнан с сервера",
                "ban" => "Бан",
                "warn" => "Предупреждение",
                "mute" => "Мут ",
                "unmute" => "Снят мут",
                _ => string.Empty
            };
        }

        string FormatToxicityLevel(uint level)
        {
            return emoteService.GetEmoteString($"$emotet{level.ToString()}");
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
