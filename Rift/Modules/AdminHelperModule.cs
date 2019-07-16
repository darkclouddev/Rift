using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
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
        readonly EconomyService economyService;
        readonly EventService eventService;
        readonly GiveawayService giveawayService;

        public AdminHelperModule(RiotService riotService,
                                 EconomyService economyService,
                                 EventService eventService,
                                 GiveawayService giveawayService)
        {
            this.riotService = riotService;
            this.economyService = economyService;
            this.eventService = eventService;
            this.giveawayService = giveawayService;
        }
        
        [Command("activestages")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task ActiveStages()
        {
            var things = await DB.Quests.GetStageIdsInProgressAsync(Context.User.Id);

            await ReplyAsync($"{things.Count.ToString()} stage(s).");
        }

        [Command("addroles")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task AddRoles()
        {
            var guildRoles = Context.Guild.Roles;

            foreach (var role in guildRoles)
            {
                await DB.Roles.AddAsync(role);
            }

            await ReplyAsync($"Added {guildRoles.Count.ToString()} role(s).");
        }

        [Command("estart")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task EventStart(string eventType)
        {
            await eventService.StartAsync(eventType, Context.User.Id);
        }

        [Command("gtstart")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task GiveawayStart(int rewardId)
        {
            await giveawayService.StartTicketGiveawayAsync(rewardId, Context.User.Id);
        }

        [Command("gastart")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task GiveawayStart(string name)
        {
            await giveawayService.StartGiveawayAsync(name, Context.User.Id);
        }

        [Command("templates")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task GetTemplates()
        {
            var templates = RiftBot.GetService<MessageService>().GetActiveTemplates();

            await ReplyAsync("**Supported templates**\n\n" +
                             $"{string.Join(',', templates.Select(x => x.Template))}");
        }

        [Command("reactions")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Reactions(ulong messageId)
        {
            var msg = (IUserMessage) await Context.Channel.GetMessageAsync(messageId);
            var text = "";

            if (msg.Reactions is null)
                text = "0";
            else
                foreach ((var emote, var reactionMetadata) in msg.Reactions)
                    text += $"**{emote}**: {reactionMetadata.ReactionCount.ToString()}\n";

            await Context.Channel.SendMessageAsync(text).ConfigureAwait(false);
        }

        [Command("exp")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Exp(uint level)
        {
            await ReplyAsync($"Level {level.ToString()}: {EconomyService.GetExpForLevel(level).ToString()} XP")
                .ConfigureAwait(false);
        }

        [Command("getprofile")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task GetProfile(IUser user)
        {
            var message = await economyService.GetUserProfileAsync(user.Id);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message).ConfigureAwait(false);
        }

        [Command("getgamestat")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task GetGameStat(IUser user)
        {
            var message = await economyService.GetUserGameStatAsync(user.Id);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message).ConfigureAwait(false);
        }

        [Command("getstat")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task GetStat(IUser user)
        {
            var message = await economyService.GetUserStatAsync(user.Id);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message).ConfigureAwait(false);
        }

        [Command("update")]
        [RequireDeveloper]
        [RateLimit(1, 10, Measure.Minutes)]
        [RequireContext(ContextType.Guild)]
        public async Task Update(IUser user)
        {
            await riotService.UpdateRankAsync(user.Id).ConfigureAwait(false);
        }

        [Command("gastart")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task StartGiveaway(string name)
        {
            await RiftBot.GetService<GiveawayService>().StartGiveawayAsync(name, Context.User.Id).ConfigureAwait(false);
        }

        [Command("la")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task ListActions(IUser user)
        {
            var msg = await RiftBot.GetService<ModerationService>().GetUserActionLogsAsync(user);
            await Context.Channel.SendIonicMessageAsync(msg).ConfigureAwait(false);
        }

        [Command("code")]
        [RequireContext(ContextType.Guild)]
        public async Task Code(IUser user)
        {
            var pendingData = await DB.PendingUsers.GetAsync(user.Id);

            if (pendingData is null)
            {
                await ReplyAsync("No data for that user.");
                return;
            }

            (var result, var code) =
                await riotService.GetThirdPartyCodeByEncryptedSummonerIdAsync(
                    pendingData.Region, pendingData.SummonedId);

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
            if (!IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.PrivateMellifluous,
                                     out var pvtMellifluous))
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
                await DB.Inventory.AddAsync(user.Id, new InventoryData {Coins = 10_000u, Spheres = 1u});

            foreach (var user in srLegendary.Members)
                await DB.Inventory.AddAsync(user.Id, new InventoryData {Coins = 50_000u, Tickets = 2u});

            var privateRoleUsers = new List<SocketGuildUser>();
            privateRoleUsers.AddRange(srBore.Members);
            privateRoleUsers.AddRange(srKayn.Members);
            privateRoleUsers.AddRange(srMellifluous.Members);
            privateRoleUsers.AddRange(srOctopus.Members);
            privateRoleUsers.AddRange(srSempai.Members);
            privateRoleUsers.AddRange(srToxic.Members);

            foreach (var user in privateRoleUsers)
                await DB.Inventory.AddAsync(user.Id, new InventoryData {Coins = 20_000u, Spheres = 1u});

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
                            var channelName = channelNames[field.Name];

                            var guildChannel = guild.Channels.FirstOrDefault(
                                x => x.Name.Equals(channelName,
                                                   StringComparison
                                                       .InvariantCultureIgnoreCase));

                            if (guildChannel is null)
                            {
                                errors.Add($"Channel ID remains undefined: {field.Name} {channelName}");
                                continue;
                            }

                            Settings.ChannelId.SetValue(field.Name, guildChannel.Id);
                            fixedRoles++;
                        }
                        else
                        {
                            errors.Add($"Channel ID remains undefined: {field.Name}");
                            continue;
                        }
                    }
                    else if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, value, out var textChannel) &&
                             !IonicClient.GetVoiceChannel(Settings.App.MainGuildId, value, out var voiceChannel))
                    {
                        errors.Add($"No channel on server: {field.Name}");
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
                    if (ulongValue == 0ul) errors.Add($"Chat parameter undefined: {field.Name}");
                }
                else if (obj is uint uintValue)
                {
                    if (uintValue == 0u) errors.Add($"Chat parameter undefined: {field.Name}");
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
                        if (ulongValue == 0ul) errors.Add($"Economy parameter undefined: {field.Name}");
                    }
                    else if (obj is uint uintValue)
                    {
                        if (uintValue == 0u) errors.Add($"Economy parameter undefined: {field.Name}");
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
                eb.WithDescription("OK 👌");
            }
            else
            {
                eb.WithColor(255, 0, 0);

                var errorList = string.Join('\n', errors);

                if (errorList.Length >= 2048)
                {
                    errorList = string.Join('\n', errors.Take(10));
                    eb.WithDescription($"**{errors.Count.ToString()} error(s), showing first 10**\n\n{errorList}");
                }
                else
                {
                    eb.WithDescription($"**{errors.Count.ToString()} error(s)**\n\n{errorList}");
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
                await Settings.SaveRolesAsync();
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

                for (var i = 0; i < remove; i++)
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

            if (Context.Client is DiscordSocketClient client) await client.SetGameAsync(RiftBot.BotStatusMessage);

            var message =
                await ReplyAsync($"Maintenance mode **{(Settings.App.MaintenanceMode ? "enabled" : "disabled")}**");
        }

        [Command("whois")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
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
            var msg = await RiftBot.GetMessageAsync("bot-about", new FormatData(212997107525746690ul));

            if (msg is null)
                return;
            
            await Context.Channel.SendIonicMessageAsync(msg);
        }

        const string addRemoveHeaderText = "Оповещение";

        [Group("give")]
        public class GiveModule : ModuleBase
        {
            const string giveText = "Основатель сервера выдал вам";

            [Command("coins")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Coins(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Coins = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotecoins {amount.ToString()}"));
            }

            [Command("tokens")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Tokens(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Tokens = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotetokens {amount.ToString()}"));
            }

            [Command("chests")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Chests(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Chests = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotechest {amount.ToString()}"));
            }

            [Command("capsules")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Capsules(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Capsules = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotecapsule {amount.ToString()}"));
            }

            [Command("spheres")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Spheres(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Spheres = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotesphere {amount.ToString()}"));
            }

            [Command("2exp")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Levels(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {DoubleExps = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emote2exp {amount.ToString()}"));
            }

            [Command("respects")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespects(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {BotRespects = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emoterespect {amount.ToString()}"));
            }

            [Command("tickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task CustomTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Tickets = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emoteticket {amount.ToString()}"));
            }
        }

        [Group("take")]
        public class TakeModule : ModuleBase
        {
            const string takeText = "Основатель сервера забрал у вас";

            [Command("coins")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Coins(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Coins = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emotecoins {amount.ToString()}"));
            }

            [Command("tokens")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Tokens(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Tokens = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emotetoken {amount.ToString()}"));
            }

            [Command("2exp")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Level(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {DoubleExps = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emote2exp {amount.ToString()}"));
            }

            [Command("chests")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Chests(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Chests = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emotechest {amount.ToString()}"));
            }

            [Command("capsules")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Capsules(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Capsules = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emotecapsule {amount.ToString()}"));
            }

            [Command("spheres")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Spheres(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Spheres = amount});

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} $emotesphere {amount.ToString()}"));
            }

            [Command("respects")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespects(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {BotRespects = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{takeText} $emoterespect {amount.ToString()}"));
            }

            [Command("tickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task CustomTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await DB.Inventory.RemoveAsync(sgUser.Id, new InventoryData {Tickets = amount});

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{takeText} $emoteticket {amount.ToString()}"));
            }
        }
    }
}
