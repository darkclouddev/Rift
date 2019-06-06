using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Economy;
using Rift.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IonicLib.Util;

namespace Rift.Modules
{
    public class EconomyModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RiotService riotService;
        readonly MessageService messageService;

        public EconomyModule(EconomyService economyService, RiotService riotService, MessageService messageService)
        {
            this.economyService = economyService;
            this.riotService = riotService;
            this.messageService = messageService;
        }

        [Command("update")]
        [RequireDeveloper]
        [RateLimit(1, 10, Measure.Minutes)]
        [RequireContext(ContextType.Guild)]
        public async Task Update(ulong userId)
        {
            await riotService.UpdateRankAsync(userId);
        }

        [Command("обновить")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage = "Запрашивать обновление ранга можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task Update()
        {
            await riotService.UpdateRankAsync(Context.User.Id);
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

        [Command("exp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task Exp(uint level)
        {
            await ReplyAsync($"Level {level.ToString()}: {EconomyService.GetExpForLevel(level).ToString()} XP");
        }

        [Command("getprofile")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetProfile(ulong userId)
        {
            var message = await economyService.GetUserProfileAsync(userId);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message);
        }

        [Command("getinventory")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetInventory(ulong userId)
        {
            var message = await economyService.GetUserInventoryAsync(userId);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message);
        }

        [Command("getgamestat")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetGameStat(ulong userId)
        {
            var message = await economyService.GetUserGameStatAsync(userId);

            if (message is null)
                return;

            await Context.Channel.SendIonicMessageAsync(message);
        }

        [Command("getstat")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task GetStat(ulong userId)
        {
            var embedResult = await economyService.GetUserStatAsync(userId);
            await Context.User.SendIonicMessageAsync(embedResult);
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

        [Command("кд")]
        [Alias("таймеры")]
        [RateLimit(1, 10.0, Measure.Seconds, RateLimitFlags.NoLimitForAdmins, ErrorMessage = "Проверять таймеры можно не чаще 1 раза в 10 секунд!")]
        [RequireContext(ContextType.Guild)]
        public async Task Cooldowns()
        {
            using (Context.Channel.EnterTypingState())
            {
                var result = await economyService.GetUserCooldownsAsync(Context.User.Id);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }

        [Group("активировать")]
        [RequireContext(ContextType.Guild)]
        public class ActivateModule : ModuleBase
        {
            readonly EconomyService economyService;

            public ActivateModule(EconomyService economyService)
            {
                this.economyService = economyService;
            }

            [Command("двойной опыт")]
            [RequireContext(ContextType.Guild)]
            public async Task DoubleExp()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await economyService.ActivateDoubleExp(Context.User.Id);
                }
            }

            [Command("уважение ботов")]
            [RequireContext(ContextType.Guild)]
            public async Task BotRespect()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await economyService.ActivateBotRespect(Context.User.Id);
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
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserBragAsync(Context.User.Id);

                await Context.Channel.SendIonicMessageAsync(message);
            }
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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Coins = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Tokens = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Chests = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Capsules = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Spheres = amount });

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{giveText} $emotesphere {amount.ToString()}"));
            }

            [Command("lvl2")]
            [RequireAdmin]
            [RequireContext(ContextType.Guild)]
            public async Task Levels(uint amount, IUser user)
            {
                if (!(user is SocketGuildUser sgUser))
                    return;

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { DoubleExps = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { BotRespects = amount });

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

                await Database.AddInventoryAsync(sgUser.Id, new InventoryData { Tickets = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Coins = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Tokens = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { DoubleExps = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Chests = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Capsules = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Spheres = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { BotRespects = amount });

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

                await Database.RemoveInventoryAsync(sgUser.Id, new InventoryData { Tickets = amount });

                await sgUser.SendEmbedAsync(
                    new EmbedBuilder()
                        .WithAuthor(addRemoveHeaderText)
                        .WithDescription($"{takeText} $emoteticket {amount.ToString()}"));
            }
        }

        [Group("открыть")]
        public class OpenModule : ModuleBase
        {
            readonly EconomyService economyService;
            
            public OpenModule(EconomyService economyService)
            {
                this.economyService = economyService;
            }

            [Command("сундук")]
            [RequireContext(ContextType.Guild)]
            public async Task Chest()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await economyService.OpenChestAsync(Context.User.Id);

                    if (message is null)
                        return;

                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("все сундуки")]
            [RequireContext(ContextType.Guild)]
            public async Task AllChests()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await economyService.OpenChestAllAsync(Context.User.Id);

                    if (message is null)
                        return;

                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("капсулу")]
            [RequireContext(ContextType.Guild)]
            public async Task Capsule()
            {
                var message = await economyService.OpenCapsuleAsync(Context.User.Id);

                await Context.Channel.SendIonicMessageAsync(message);
            }

            [Command("сферу")]
            [RequireContext(ContextType.Guild)]
            public async Task Sphere()
            {
                var embed = await economyService.OpenSphereAsync(Context.User.Id);

                await Context.Channel.SendIonicMessageAsync(embed);
            }
        }

        [Command("купить")]
        [RequireContext(ContextType.Guild)]
        public async Task Buy(uint id)
        {
            using (Context.Channel.EnterTypingState())
            {
                var storeItem = Store.GetShopItemById(id);

                if (storeItem is null)
                {
                    await Context.Channel.SendMessageAsync("Данный номер отсутствует в магазине.");
                    return;
                }

                var result = await economyService.StorePurchaseAsync(Context.User.Id, storeItem);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }

        [Command("магазин")]
        public async Task Shop()
        {
            await Context.User.SendEmbedAsync(Store.Embed);
        }
    }
}
