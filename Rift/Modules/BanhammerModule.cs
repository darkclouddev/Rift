using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Preconditions;
using Rift.Services;

using IonicLib;
using IonicLib.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Rift.Modules
{
    public class BanhammerModule : RiftModuleBase
    {
        readonly RoleService roleService;

        public BanhammerModule(RoleService roleSvc)
        {
            roleService = roleSvc;
        }

        [Command("kick")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Kick(IUser user)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync($"Не удалось найти пользователя на сервере!");
                return;
            }

            if (RiftBot.IsAdmin(sgUser) || RiftBot.IsModerator(sgUser))
            {
                await Context.User.SendMessageAsync($"Огонь по своим недопустим!");
                return;
            }

            await sgUser.KickAsync();

            var redColor = new Color(255, 0, 0);

            await chatChannel.SendEmbedAsync(AdminEmbeds.Kick(sgUser));
        }

        [Command("ban")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Ban(IUser user)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync($"Не удалось найти пользователя на сервере!");
                return;
            }

            if (RiftBot.IsAdmin(sgUser) || RiftBot.IsModerator(sgUser))
            {
                await Context.User.SendMessageAsync($"Огонь по своим недопустим!");
                return;
            }

            await Context.Guild.AddBanAsync(sgUser, 1, $"Banned by {Context.User.ToString()}");

            await chatChannel.SendEmbedAsync(AdminEmbeds.Ban(sgUser));
        }

        [Command("mute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(string time, IUser user, string reason)
        {
            if (!Int32.TryParse(time.Remove(time.Length - 1), out int timeInt))
            {
                await Context.User.SendMessageAsync($"Неверный формат времени: \"{time}\"");
                return;
            }

            if (String.IsNullOrEmpty(reason))
            {
                await Context.User.SendMessageAsync($"Не указана причина.");
                return;
            }

            TimeSpan ts;

            char timeMod = time.Last();

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
                    await Context.User.SendMessageAsync($"Неверный модификатор времени: \"{timeMod}\"");
                    return;
            }

            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync($"Не удалось найти пользователя на сервере.");
                return;
            }

            await roleService.AddTempRoleAsync(user.Id, Settings.RoleId.Muted, ts, $"Muted by {Context.User.Id} with reason: {reason}");

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            await chatChannel.SendEmbedAsync(AdminEmbeds.ChatMute(sgUser, reason));
            await sgUser.SendEmbedAsync(AdminEmbeds.DMMute(ts, reason));
        }

        [Command("unmute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Unmute(IUser user)
        {
            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync($"Не удалось найти пользователя.");
                return;
            }

            await roleService.RemoveTempRoleAsync(sgUser.Id, Settings.RoleId.Muted);
        }

        [Command("warn")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Warn(IUser user)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync($"Не удалось найти пользователя на сервере!");
                return;
            }

            await chatChannel.SendEmbedAsync(AdminEmbeds.Warn(sgUser));
        }
    }
}
