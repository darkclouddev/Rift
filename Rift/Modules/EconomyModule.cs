using System;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Economy;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Rift.Modules
{
    public class EconomyModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RiotService riotService;
        readonly DatabaseService databaseService;
        readonly MessageService messageService;

        public EconomyModule(EconomyService economyService,
                             RiotService riotService,
                             DatabaseService databaseService,
                             MessageService messageService)
        {
            this.economyService = economyService;
            this.riotService = riotService;
            this.databaseService = databaseService;
            this.messageService = messageService;
        }

        [Command("update")]
        [RequireDeveloper]
        [RateLimit(1, 10, Measure.Minutes)]
        [RequireContext(ContextType.Guild)]
        public async Task Update(ulong userId)
        {
            //var eb = new EmbedBuilder()
            //	.WithAuthor("Оповещения")
            //	.WithDescription($"Недоступно до обновления.");

            //await Context.User.SendEmbedAsync(eb);
            //return;

            await riotService.UpdateRankAsync(userId);
        }

        [Command("обновить")]
        [RateLimit(1, 10, Measure.Minutes)]
        [RequireContext(ContextType.Guild)]
        public async Task Update()
        {
            //var eb = new EmbedBuilder()
            //	.WithAuthor("Оповещения")
            //	.WithDescription($"Недоступно до обновления.");

            //await Context.User.SendEmbedAsync(eb);
            //return;

            await riotService.UpdateRankAsync(Context.User.Id);
        }

        [Group("gift")]
        public class SpecialGiftModule : ModuleBase
        {
            readonly EconomyService economyService;

            public SpecialGiftModule(EconomyService economyService)
            {
                this.economyService = economyService;
            }

            [Command("stream")]
            [RequireStreamer]
            [RequireContext(ContextType.Guild)]
            public async Task Stream(IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await GiftAsync(Context.User.Id, sgUser.Id, GiftSource.Streamer);
            }

            [Command("mod")]
            [RequireModerator]
            [RequireContext(ContextType.Guild)]
            public async Task Moderator(IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await GiftAsync(Context.User.Id, sgUser.Id, GiftSource.Moderator);
            }

            [Command("voice")]
            [RequireModerator]
            [RequireContext(ContextType.Guild)]
            public async Task Voice(IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await GiftAsync(Context.User.Id, sgUser.Id, GiftSource.Voice);
            }

            async Task GiftAsync(ulong fromId, ulong toId, GiftSource giftSource)
            {
                await economyService.GiftSpecialAsync(fromId, toId, giftSource);
            }
        }

        [Command("активные")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Active()
        {
            await EconomyService.ShowActiveUsersAsync();
        }

        [Command("богатые")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task RichBitch()
        {
            await EconomyService.ShowRichUsersAsync();
        }

        [Command("донатеры")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Donators()
        {
            var topTen =
                await databaseService.GetTopTenDonatesAsync(x => !(IonicClient.GetGuildUserById(Settings.App.MainGuildId,
                                                                                           x.UserId) is null));

            if (topTen.Length == 0)
                return;

            await Context.Channel.SendEmbedAsync(DonateEmbeds.StatisticEmbed(topTen));
        }

        [Command("exp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task Exp(uint level)
        {
            await ReplyAsync($"Level {level}: {EconomyService.GetExpForLevel(level)} XP");
        }

        [Command("getprofile")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetProfile(ulong userId)
        {
            var embedResult = await economyService.GetUserProfileAsync(userId);

            await Context.User.SendEmbedAsync(embedResult);
        }

        [Command("getinventory")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetInventory(ulong userId)
        {
            var embedResult = await economyService.GetUserInventoryAsync(userId);

            await Context.User.SendEmbedAsync(embedResult);
        }

        [Command("getgamestat")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetGameStat(ulong userId)
        {
            //var eb = new EmbedBuilder()
            //	.WithAuthor("Оповещения")
            //	.WithDescription($"Недоступно до обновления.");

            //await Context.User.SendEmbedAsync(eb);
            //return;

            var embedResult = await economyService.GetUserGameStatAsync(userId);

            await Context.User.SendEmbedAsync(embedResult);
        }

        [Command("getstat")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetStat(ulong userId)
        {
            var embedResult = await economyService.GetUserStatAsync(userId);
            await Context.User.SendEmbedAsync(embedResult);
        }

        [Command("профиль")]
        [RequireContext(ContextType.Guild)]
        public async Task Profile()
        {
            using (Context.Channel.EnterTypingState())
            {
                await GetProfile(Context.User.Id);
            }
        }

        [Group("активировать")]
        [RequireContext(ContextType.Guild)]
        public class ActivateModule : ModuleBase
        {
            readonly EconomyService economy;

            public ActivateModule(EconomyService service)
            {
                economy = service;
            }

            [Command("двойной опыт")]
            [RequireContext(ContextType.Guild)]
            public async Task DoubleExp()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await economy.ActivateDoubleExp(Context.User.Id);
                }
            }

            [Command("уважение ботов")]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespect()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await economy.ActivateBotRespect(Context.User.Id);
                }
            }
        }

        [Command("инвентарь")]
        [RequireContext(ContextType.Guild)]
        public async Task Inventory()
        {
            using (Context.Channel.EnterTypingState())
            {
                await GetInventory(Context.User.Id);
            }
        }

        [Command("игровой профиль")]
        [RateLimit(1, 10, Measure.Minutes)]
        [RequireContext(ContextType.Guild)]
        public async Task GameStat()
        {
            using (Context.Channel.EnterTypingState())
            {
                await GetGameStat(Context.User.Id);
            }
        }

        [Command("статистика")]
        [RequireContext(ContextType.Guild)]
        public async Task Stat()
        {
            using (Context.Channel.EnterTypingState())
            {
                await GetStat(Context.User.Id);
            }
        }

        [Command("похвастаться")]
        [RequireContext(ContextType.Guild)]
        public async Task Brag()
        {
            //var eb = new EmbedBuilder()
            //	.WithAuthor("Оповещения")
            //	.WithDescription($"Недоступно до обновления.");

            //await Context.User.SendEmbedAsync(eb);
            //return;

            using (Context.Channel.EnterTypingState())
            {
                (var state, var embed) = await economyService.GetUserBragAsync(Context.User.Id);

                if (state == BragResult.Error)
                {
                    await Context.User.SendEmbedAsync(embed);
                    return;
                }

                var msg = await Context.Channel.SendEmbedAsync(embed);
                messageService.TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(15)));
            }
        }

        const string addRemoveHeaderText = "Оповещение";

        [Group("give")]
        public class GiveModule : ModuleBase
        {
            const string giveText = "Основатель сервера выдал вам";

            readonly DatabaseService databaseService;

            public GiveModule(DatabaseService databaseService)
            {
                this.databaseService = databaseService;
            }

            [Command("coins")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Coins(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, coins: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.Coin} {amount}"));
            }

            [Command("tokens")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Tokens(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, tokens: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.Token} {amount}"));
            }

            [Command("chests")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Chests(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, chests: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.Chest} {amount}"));
            }

            [Command("capsules")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Capsules(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, capsules: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.Capsule} {amount}"));
            }

            [Command("spheres")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Spheres(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, spheres: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.Sphere} {amount}"));
            }

            [Command("lvl2")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Levels(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, doubleExps: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.PowerupDoubleExperience} {amount}"));
            }

            [Command("respects")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespects(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, respects: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.BotRespect} {amount}"));
            }

            [Command("ctickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task CustomTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, usualTickets: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.UsualTickets} {amount}"));
            }

            [Command("gtickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task GiveawayTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.AddInventoryAsync(sgUser.Id, rareTickets: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{giveText} {Settings.Emote.RareTickets} {amount}"));
            }
        }

        [Group("take")]
        public class TakeModule : ModuleBase
        {
            const string takeText = "Основатель сервера забрал у вас";

            readonly DatabaseService databaseService;

            public TakeModule(DatabaseService databaseService)
            {
                this.databaseService = databaseService;
            }

            [Command("coins")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Coins(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, coins: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.Coin} {amount}"));
            }

            [Command("tokens")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Tokens(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, tokens: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.Token} {amount}"));
            }

            [Command("lvl2")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Level(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, doubleExps: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.PowerupDoubleExperience} {amount}"));
            }

            [Command("chests")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Chests(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, chests: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.Chest} {amount}"));
            }

            [Command("capsules")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Capsules(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, capsules: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.Capsule} {amount}"));
            }

            [Command("spheres")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Spheres(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, spheres: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.Sphere} {amount}"));
            }

            [Command("respects")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespects(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, respects: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.BotRespect} {amount}"));
            }

            [Command("ctickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task CustomTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, usualTickets: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.UsualTickets} {amount}"));
            }

            [Command("gtickets")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task GiveawayTickets(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, rareTickets: amount);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"{takeText} {Settings.Emote.RareTickets} {amount}"));
            }

            [Command("all")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task All(IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await databaseService.RemoveInventoryAsync(sgUser.Id, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue,
                                                           UInt32.MaxValue, UInt32.MaxValue,
                                                           UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue,
                                                           UInt32.MaxValue);

                await sgUser.SendEmbedAsync(new EmbedBuilder()
                                            .WithAuthor(addRemoveHeaderText)
                                            .WithDescription($"Основатель сервера очистил ваш инвентарь."));
            }
        }

        [Group("открыть")]
        public class OpenModule : ModuleBase
        {
            readonly EconomyService economyService;
            readonly DatabaseService databaseService;

            public OpenModule(EconomyService economyService, DatabaseService databaseService)
            {
                this.economyService = economyService;
                this.databaseService = databaseService;
            }

            [Command("сундук")]
            [RequireContext(ContextType.Guild)]
            public async Task Chest()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var (state, embed) = await economyService.OpenChestAsync(Context.User.Id);

                    await PostOpenAsync(state, embed);
                }
            }

            [Command("все сундуки")]
            [RequireContext(ContextType.Guild)]
            public async Task AllChests()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var (state, embed) = await economyService.OpenChestAllAsync(Context.User.Id);

                    await PostOpenAsync(state, embed);
                }
            }

            async Task PostOpenAsync(OpenChestResult state, Embed embed)
            {
                switch (state)
                {
                    case OpenChestResult.Error:

                        await Context.User.SendEmbedAsync(embed);
                        break;

                    case OpenChestResult.NoChests:

                        await Context.User.SendEmbedAsync(embed);
                        break;
                }
            }

            [Command("капсулу")]
            [RequireContext(ContextType.Guild)]
            public async Task Capsule()
            {
                (var state, var embed) = await economyService.OpenCapsuleAsync(Context.User.Id);

                await Context.User.SendEmbedAsync(embed);
            }

            [Command("сферу")]
            [RequireContext(ContextType.Guild)]
            public async Task Sphere()
            {
                (var state, var embed) = await economyService.OpenSphereAsync(Context.User.Id);

                await Context.User.SendEmbedAsync(embed);
            }
        }

        [Group("купить")]
        public class BuyModule : ModuleBase
        {
            readonly EconomyService economy;

            public BuyModule(EconomyService service)
            {
                economy = service;
            }

            [Command]
            [RequireContext(ContextType.Guild)]
            public async Task Default(uint id)
            {
                using (Context.Channel.EnterTypingState())
                {
                    var storeItem = Store.GetShopItemById(id);

                    if (storeItem is null)
                    {
                        await Context.User.SendMessageAsync($"Данный номер отсутствует в магазине.");
                        return;
                    }

                    var result = await economy.StorePurchaseAsync(Context.User.Id, storeItem);

                    switch (result.Item1)
                    {
                        default:

                            await Context.User.SendEmbedAsync(result.Item2);
                            break;
                    }
                }
            }
        }

        [Group("подарить")]
        public class GiftModule : ModuleBase
        {
            readonly EconomyService economy;

            public GiftModule(EconomyService service)
            {
                economy = service;
            }

            [Command]
            [RequireContext(ContextType.Guild)]
            public async Task Default(uint id, IUser user)
            {
                using (Context.Channel.EnterTypingState())
                {
                    if (user is null || !(user is SocketGuildUser sgUser))
                    {
                        await Context.User.SendMessageAsync($"Пользователь не найден!");
                        return;
                    }

                    await SendGiftAsync((SocketGuildUser) Context.User, sgUser, id);
                }
            }

            async Task SendGiftAsync(SocketGuildUser fromSgUser, SocketGuildUser toSgUser, uint id)
            {
                var result = await economy.GiftAsync(fromSgUser, toSgUser, id);

                if (result.Item1 != GiftResult.Success)
                {
                    switch (result.Item1)
                    {
                        default:

                            await Context.User.SendEmbedAsync(result.Item2);
                            break;
                    }
                }
            }
        }

        [Group("атаковать")]
        public class AttackModule : ModuleBase
        {
            readonly EconomyService economy;

            public AttackModule(EconomyService service)
            {
                economy = service;
            }

            [Command]
            [RequireContext(ContextType.Guild)]
            public async Task Default([Remainder] string mention)
            {
                using (Context.Channel.EnterTypingState())
                {
                    var sgTarget = await MentionHelper.ResolveFirstMentionedUser(Context);

                    if (sgTarget == null || !(Context.User is SocketGuildUser sgUser))
                    {
                        await Context.User.SendMessageAsync($"Пользователь не найден!");
                        return;
                    }

                    await AttackAsync(sgUser, sgTarget);
                }
            }

            async Task AttackAsync(SocketGuildUser sgAttacker, SocketGuildUser sgTarget)
            {
                var result = await economy.AttackAsync(sgAttacker, sgTarget);

                if (result.Item1 == AttackResult.Success)
                    return;

                switch (result.Item1)
                {
                    default:

                        await Context.User.SendEmbedAsync(result.Item2);

                        break;
                }
            }
        }

        [Command("атаки")]
        public async Task Attacks()
        {
            await Context.User.SendEmbedAsync(AttackEmbeds.Help);
        }

        [Command("магазин")]
        public async Task Shop()
        {
            await Context.User.SendEmbedAsync(Store.Embed);
        }

        [Command("подарки")]
        public async Task Gifts()
        {
            await Context.User.SendEmbedAsync(Gift.Embed);
        }

        [Command("достижения")]
        [RequireContext(ContextType.Guild)]
        public async Task Achievements()
        {
            using (Context.Channel.EnterTypingState())
            {
                await EconomyService.GetAchievements(Context.User.Id);
            }
        }

        [Command("платные роли")]
        [RequireContext(ContextType.Guild)]
        public async Task DonateRoles()
        {
            using (Context.Channel.EnterTypingState())
            {
                await Context.User.SendEmbedAsync(RoleEmbeds.DonatedRoles);
            }
        }
    }
}
