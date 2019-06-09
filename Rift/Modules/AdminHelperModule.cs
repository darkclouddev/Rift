using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Message;
using Rift.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IonicLib;
using IonicLib.Util;

namespace Rift.Modules
{
    public class AdminHelperModule : RiftModuleBase
    {
        readonly RiotService riotService;
        readonly ModerationService moderationService;

        public AdminHelperModule(RiotService riotService, ModerationService moderationService)
        {
            this.riotService = riotService;
            this.moderationService = moderationService;
        }

        [Command("la")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task ListActions(IUser user)
        {
            var msg = await moderationService.GetUserActionLogsAsync(user);
            await Context.Channel.SendIonicMessageAsync(msg);
        }

        [Command("code")]
        [RequireContext(ContextType.DM)]
        public async Task Code(ulong userId)
        {
            var pendingData = await Database.GetPendingUserAsync(userId);

            if (pendingData is null)
            {
                await ReplyAsync($"No data for that user.");
                return;
            }

            (var result, var code) = await riotService.GetThirdPartyCodeByEncryptedSummonerIdAsync(pendingData.Region, pendingData.SummonedId);

            if (result != RequestResult.Success)
            {
                await ReplyAsync($"Failed to obtain confirmation code!");
                return;
            }

            await ReplyAsync($"Expected code: \"{pendingData.ConfirmationCode}\"\nActual code: \"{code}\"");
        }

        [Command("донат подарки")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task DonateRewards()
        {
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.Absolute, out var absoluteRole))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.Legendary, out var legendaryRole))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateBore, out var pvtBore))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateKayn, out var pvtKayn))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateMellifluous, out var pvtMellifluous))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateOctopusMom, out var pvtOctopus))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateSempai, out var pvtSempai))
                return;
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateToxic, out var pvtToxic))
                return;

            if (!(absoluteRole is SocketRole srAbsolute))
                return;
            if (!(legendaryRole is SocketRole srLegendary))
                return;
            if (!(pvtBore is SocketRole srBore))
                return;
            if (!(pvtKayn is SocketRole srKayn))
                return;
            if (!(pvtMellifluous is SocketRole srMellifluous))
                return;
            if (!(pvtOctopus is SocketRole srOctopus))
                return;
            if (!(pvtSempai is SocketRole srSempai))
                return;
            if (!(pvtToxic is SocketRole srToxic))
                return;

            foreach (var user in srAbsolute.Members)
            {
                await Database.AddInventoryAsync(user.Id, new InventoryData { Coins = 10_000u, Spheres = 1u });
            }

            foreach (var user in srLegendary.Members)
            {
                await Database.AddInventoryAsync(user.Id, new InventoryData { Coins = 50_000u, Tickets = 2u });
            }

            var privateRoleUsers = new List<SocketGuildUser>();
            privateRoleUsers.AddRange(srBore.Members);
            privateRoleUsers.AddRange(srKayn.Members);
            privateRoleUsers.AddRange(srMellifluous.Members);
            privateRoleUsers.AddRange(srOctopus.Members);
            privateRoleUsers.AddRange(srSempai.Members);
            privateRoleUsers.AddRange(srToxic.Members);

            foreach (var user in privateRoleUsers)
            {
                await Database.AddInventoryAsync(user.Id, new InventoryData { Coins = 20_000u, Spheres = 1u });
            }

            await Context.User.SendMessageAsync($"Выдача подарков завершена!\n\n"
                                                + $"{srAbsolute.Name}: {srAbsolute.Members.Count().ToString()}\n"
                                                + $"{srLegendary.Name}: {srLegendary.Members.Count().ToString()}\n"
                                                + $"Личные роли: {privateRoleUsers.Count.ToString()}");
        }

        [Command("selftest")]
        [RequireDeveloper]
        public async Task SelfTest()
        {
            var skipChecks = false; // TODO: Make it configurable

            var errors = new List<string>();
            var fixedRoles = 0u;

            var eb = new RiftEmbed().WithTitle("Self-test");

            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
            {
                errors.Add($"Guild is null: {nameof(Settings.App.MainGuildId)}");
                skipChecks = true;
            }

            var channelNames = Settings.ChannelId.GetNames();

            foreach (var field in Settings.ChannelId.GetType().GetProperties())
            {
                if (skipChecks)
                    break;

                if (field.GetValue(Settings.ChannelId, null) is ulong value)
                {
                    if (value == 0ul)
                    {
                        if (channelNames.ContainsKey(field.Name))
                        {
                            var (categoryName, channelName) = channelNames[field.Name];

                            var guildCategory = guild.CategoryChannels.FirstOrDefault(x =>
                                x.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));

                            if (guildCategory is null)
                            {
                                errors.Add($"No such category: {categoryName}");
                                continue;
                            }

                            var guildChannel = guildCategory.Channels.FirstOrDefault(x =>
                                x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));

                            if (guildChannel is null)
                            {
                                errors.Add($"Channel ID remains undefined: {field.Name} ({categoryName}|{channelName})");
                                continue;
                            }

                            Settings.RoleId.SetValue(field.Name, guildChannel.Id);
                            fixedRoles++;
                        }
                        else
                        {
                            errors.Add($"Role ID undefined: {field.Name}");
                            continue;
                        }
                    }
                    else if (!IonicClient.GetRole(Settings.App.MainGuildId, value, out var channel))
                    {
                        errors.Add($"No role on server: {field.Name}");
                    }
                }
            }

            foreach (var field in Settings.ChannelId.GetType().GetProperties())
            {
                if (skipChecks)
                    break;

                if (field.GetValue(Settings.ChannelId) is ulong value)
                {
                    if (value == 0ul)
                    {
                        errors.Add($"Channel ID is undefined: {field.Name}");
                    }
                    else if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, value, out var channel))
                    {
                        errors.Add($"No text channel on server: {field.Name}");
                    }
                }
            }

            foreach (var field in Settings.Chat.GetType().GetProperties())
            {
                if (skipChecks)
                    break;

                var obj = field.GetValue(Settings.Chat);

                if (obj is ulong ulongValue)
                {
                    if (ulongValue == 0ul)
                    {
                        errors.Add($"Chat parameter undefined: {field.Name}");
                    }
                }
                else if (obj is uint uintValue)
                {
                    if (uintValue == 0u)
                    {
                        errors.Add($"Chat parameter undefined: {field.Name}");
                    }
                }
            }

            foreach (var field in Settings.Economy.GetType().GetProperties())
            {
                if (skipChecks)
                    break;

                try
                {
                    var obj = field.GetValue(Settings.Economy);

                    if (obj is ulong ulongValue)
                    {
                        if (ulongValue == 0ul)
                        {
                            errors.Add($"Economy parameter undefined: {field.Name}");
                        }
                    }
                    else if (obj is uint uintValue)
                    {
                        if (uintValue == 0u)
                        {
                            errors.Add($"Economy parameter undefined: {field.Name}");
                        }
                    }
                }
                catch (TargetInvocationException ex)
                {
                    errors.Add($"\"{field.Name}\" invokation failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    errors.Add($"Economy object exception failed: {ex.Message}");
                }
            }

            var roleNames = Settings.RoleId.GetNames();

            foreach (var field in Settings.RoleId.GetType().GetProperties())
            {
                if (skipChecks)
                    break;

                if (field.GetValue(Settings.RoleId, null) is ulong value)
                {
                    if (value == 0ul)
                    {
                        if (roleNames.ContainsKey(field.Name))
                        {
                            var serverRole = guild.Roles.FirstOrDefault(x => x.Name.Contains(roleNames[field.Name]));

                            if (serverRole is null)
                            {
                                errors.Add($"Role ID undefined: {field.Name}");
                                continue;
                            }

                            Settings.RoleId.SetValue(field.Name, serverRole.Id);
                            fixedRoles++;
                        }
                        else
                        {
                            errors.Add($"Role ID undefined: {field.Name}");
                            continue;
                        }
                    }
                    else if (!IonicClient.GetRole(Settings.App.MainGuildId, value, out var channel))
                    {
                        errors.Add($"No role on server: {field.Name}");
                    }
                }
            }

            if (errors.Count == 0)
            {
                eb.WithColor(0, 255, 0);
                eb.WithDescription("OK");
            }
            else
            {
                eb.WithColor(255, 0, 0);

                var errorList = String.Join('\n', errors);

                if (errorList.Length >= 1024)
                {
                    errorList = string.Join('\n', errors.Take(10));
                    eb.AddField($"{errors.Count.ToString()} error(s)", "Displaying first 10\n\n" + errorList);
                }
                else
                {
                    eb.AddField($"{errors.Count.ToString()} error(s)", errorList);
                }
            }

            await Context.Channel.SendIonicMessageAsync(new IonicMessage(eb));

            if (fixedRoles > 0u)
            {
                var embedMsg = new RiftEmbed()
                               .WithColor(255, 255, 0)
                               .WithAuthor("Self-test")
                               .WithDescription($"Automatically resolved {fixedRoles.ToString()} roles.");

                await Context.Channel.SendIonicMessageAsync(new IonicMessage(embedMsg));
            }
        }

        [Command("listroles")]
        [RequireDeveloper]
        public async Task ListRoles()
        {
            var roles = new Queue<IRole>(Context.Guild.Roles);

            var amount = roles.Count;
            var page = 0;

            while (amount > 0)
            {
                ++page;

                var eb = new EmbedBuilder()
                         .WithAuthor($"Server roles")
                         .WithFooter($"Page {page.ToString()}");

                var ids = new List<ulong>();
                var names = new List<string>();

                var remove = amount >= 20 ? 20 : amount;

                for (int i = 0; i < remove; i++)
                {
                    var role = roles.Dequeue();

                    if (role == Context.Guild.EveryoneRole)
                        continue;

                    ids.Add(role.Id);
                    names.Add(role.Name);
                }

                eb.AddField("Role ID", string.Join('\n', ids), true);
                eb.AddField("Name", string.Join('\n', names), true);

                await Context.Channel.SendEmbedAsync(eb);

                amount -= remove;
            }
        }

        [Command("maintenance")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Maintenance()
        {
            Settings.App.MaintenanceMode = !Settings.App.MaintenanceMode;

            if (Context.Client is DiscordSocketClient client)
            {
                await client.SetGameAsync(RiftBot.BotStatusMessage);
            }

            var message = await ReplyAsync($"Maintenance mode **{(Settings.App.MaintenanceMode ? "enabled" : "disabled")}**");
        }

        [Command("whois")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task WhoIs(ulong userId)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (user is null)
            {
                await ReplyAsync($"Пользователя с таким ID нет на сервере.");
                return;
            }

            await ReplyAsync($"Пользователь найден: {user.Mention} ({user.Username}#{user.Discriminator})");
        }

        [Command("ff")]
        [RequireAdmin]
        public async Task FF()
        {
            await Context.Message.DeleteAsync();
            //await ReplyAsync(embed: AdminEmbeds.ShutDown); // TODO

            IonicClient.TokenSource.Cancel();
        }

        [Command("reboot")]
        [RequireAdmin]
        public async Task Reboot()
        {
            RiftBot.ShouldReboot = true;

            await Context.Message.DeleteAsync();
            await ReplyAsync("Перезапускаюсь");

            IonicClient.TokenSource.Cancel();
        }

        [Command("listemotes")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Emotes()
        {
            var emotes = Context.Guild.Emotes.ToList();

            var re = new RiftEmbed()
                .WithAuthor("Server emotes")
                .AddField("Emote", string.Join('\n', emotes.Select(x => x.Name)), true)
                .AddField("ID", string.Join('\n', emotes.Select(x => x.Id)), true);

            await Context.Channel.SendIonicMessageAsync(new IonicMessage(re));
        }

        [Command("about")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task AppStatus()
        {
            await RiftBot.SendChatMessageAsync("bot-about", new FormatData(212997107525746690ul));
        }
    }
}
