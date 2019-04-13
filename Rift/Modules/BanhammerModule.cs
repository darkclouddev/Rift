using System;
using System.Linq;
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
        public async Task Kick(IUser user, [Remainder]String reason)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Modchat, out var modChannel))
                return;

            if (String.IsNullOrEmpty(reason))
            {
                await Context.User.SendMessageAsync("Не указана причина.");
                return;
            }
            
            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync("Не удалось найти пользователя на сервере!");
                return;
            }

            if (RiftBot.IsAdmin(sgUser) || RiftBot.IsModerator(sgUser))
            {
                await Context.User.SendMessageAsync("Огонь по своим недопустим!");
                return;
            }

            await sgUser.KickAsync();
            await chatChannel.SendEmbedAsync(BanhammerEmbeds.Kick(sgUser));
            await modChannel.SendEmbedAsync(BanhammerEmbeds.KickLog(sgUser, Context.User.ToString(), reason));
        }

        [Command("ban")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Ban(IUser user, [Remainder]String reason)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Modchat, out var modChannel))
                return;

            if (String.IsNullOrEmpty(reason))
            {
                await Context.User.SendMessageAsync("Не указана причина.");
                return;
            }

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

            await Context.Guild.AddBanAsync(sgUser, 1, $"Banned by {Context.User}");

            await chatChannel.SendEmbedAsync(BanhammerEmbeds.Ban(sgUser));
            await modChannel.SendEmbedAsync(BanhammerEmbeds.BanLog(sgUser, Context.User.ToString(), reason));
        }

        [Command("mute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(IUser user, String time, String reason)
        {
            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync("Не удалось найти пользователя на сервере.");
                return;
            }
            
            if (!Int32.TryParse(time.Remove(time.Length - 1), out var timeInt))
            {
                await Context.User.SendMessageAsync($"Неверный формат времени: \"{time}\"");
                return;
            }

            if (String.IsNullOrEmpty(reason))
            {
                await Context.User.SendMessageAsync("Не указана причина.");
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
                    await Context.User.SendMessageAsync($"Неверный модификатор времени: \"{timeMod.ToString()}\"");
                    return;
            }

            await roleService.AddTempRoleAsync(user.Id, Settings.RoleId.Muted, ts, $"Muted by {Context.User.Id.ToString()} with reason: {reason}");

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;
            
            await chatChannel.SendEmbedAsync(BanhammerEmbeds.ChatMute(sgUser, reason));
            await sgUser.SendEmbedAsync(BanhammerEmbeds.DMMute(ts, reason));
            
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Modchat, out var modChannel))
                return;
            
            await modChannel.SendEmbedAsync(BanhammerEmbeds.MuteLog(sgUser, Context.User.ToString(), ts, reason));
        }

        [Command("unmute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Unmute(IUser user, String reason)
        {
            if (!(user is SocketGuildUser sgUser))
            {
                await Context.User.SendMessageAsync("Не удалось найти пользователя.");
                return;
            }
            
            if (String.IsNullOrEmpty(reason))
            {
                await Context.User.SendMessageAsync("Не указана причина.");
                return;
            }

            await roleService.RemoveTempRoleAsync(sgUser.Id, Settings.RoleId.Muted);
            
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Modchat, out var modChannel))
                return;
            
            await modChannel.SendEmbedAsync(BanhammerEmbeds.UnmuteLog(sgUser, Context.User.ToString(), reason));
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
                await Context.User.SendMessageAsync("Не удалось найти пользователя на сервере!");
                return;
            }

            await chatChannel.SendEmbedAsync(BanhammerEmbeds.Warn(sgUser));
        }
    }
}
