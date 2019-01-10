using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Role;

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

            TempRole tempRole = new TempRole(user.Id, Settings.RoleId.Muted, ts);

            await roleService.AddTempRoleAsync(tempRole);

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

            await roleService.SetExpiredAsync(sgUser.Id, Settings.RoleId.Muted);
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

        [Command("banlist")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Banlist()
        {
            var banlist = roleService.GetMutes();

            if (banlist == null || banlist.Count() == 0)
            {
                var msg = await ReplyAsync($"Банлист пуст (пока что).");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Список провинившихся");
            sb.AppendLine();

            foreach (var ban in banlist)
            {
                var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, ban.UserId);
                var value = user != null ? $"{user.Username}#{user.Discriminator}" : ban.UserId.ToString();

                sb.AppendLine($"{value} до {ban.UntilTimestamp:(dd/MM/yy HH:mm)}");
            }

            await ReplyAsync(sb.ToString());
        }
    }
}
