using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Data.Models;
using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Services.Store;

using IonicLib;

using Newtonsoft.Json;

namespace Rift.Services
{
    public class StoreService
    {
        public static EventHandler<BoughtChestsEventArgs> BoughtChests;
        public static EventHandler<RolesPurchasedEventArgs> RolesPurchased;

        static readonly SemaphoreSlim MutexItemStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexRoleStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexBackStore = new SemaphoreSlim(1);

        List<StoreItem> itemShopList;

        List<StoreItem> ItemShopList
        {
            get
            {
                if (itemShopList is null)
                    itemShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, "Сундуки (1 шт.)", StoreItemType.Chest, 1u, 2_000u, Currency.Coins),
                        new StoreItem(2u, "Сундуки (6 шт.)", StoreItemType.Chest, 6u, 11_000u, Currency.Coins),
                        new StoreItem(3u, "Уважение ботов", StoreItemType.BotRespect, 1u, 24_000u, Currency.Coins),
                        new StoreItem(4u, "Сфера", StoreItemType.Sphere, 1u, 40_000u, Currency.Coins),
                    };

                return itemShopList;
            }
        }

        List<StoreItem> roleShopList;

        List<StoreItem> RoleShopList
        {
            get
            {
                if (roleShopList is null)
                {
                    try
                    {
                        roleShopList = new List<StoreItem>
                        {
                            new StoreItem(1u, StoreItemType.PermanentRole, 28, 50_000u, Currency.Coins),
                            new StoreItem(2u, StoreItemType.PermanentRole, 15, 200_000u, Currency.Coins),
                            new StoreItem(3u, StoreItemType.PermanentRole, 45, 200_000u, Currency.Coins),
                            new StoreItem(4u, StoreItemType.PermanentRole, 75, 300_000u, Currency.Coins),
                            new StoreItem(5u, StoreItemType.PermanentRole, 35, 400_000u, Currency.Coins),
                            new StoreItem(6u, StoreItemType.PermanentRole, 57, 600_000u, Currency.Coins),
                            new StoreItem(7u, StoreItemType.PermanentRole, 19, 700_000u, Currency.Coins),
                            new StoreItem(8u, StoreItemType.PermanentRole, 55, 750_000u, Currency.Coins),
                            new StoreItem(9u, StoreItemType.PermanentRole, 38, 800_000u, Currency.Coins),
                            new StoreItem(10u, StoreItemType.PermanentRole, 27, 250u, Currency.Tokens),
                            new StoreItem(11u, StoreItemType.PermanentRole, 37, 250u, Currency.Tokens),
                            new StoreItem(12u, StoreItemType.PermanentRole, 82, 300u, Currency.Tokens),
                            new StoreItem(13u, StoreItemType.PermanentRole, 59, 350u, Currency.Tokens),
                            new StoreItem(14u, StoreItemType.PermanentRole, 63, 400u, Currency.Tokens),
                            new StoreItem(15u, StoreItemType.PermanentRole, 7, 500u, Currency.Tokens),
                        };
                    }
                    catch (Exception ex)
                    {
                        RiftBot.Log.Error("Failed to get one or more store roles!");
                        RiftBot.Log.Error(ex);
                        roleShopList = null;
                    }
                }

                return roleShopList;
            }
        }

        List<StoreItem> backgroundShopList;

        List<StoreItem> BackgroundShopList
        {
            get
            {
                if (backgroundShopList is null)
                {
                    var backgrounds = Task.Run(async () => await DB.ProfileBackgrounds.GetAllAsync()).Result;

                    backgroundShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, $"[{backgrounds[0].Name}]({backgrounds[0].Url})",
                            StoreItemType.ProfileBackground, backgrounds[0].Id, 70_000u, Currency.Coins),
                        new StoreItem(2u, $"[{backgrounds[1].Name}]({backgrounds[1].Url})",
                            StoreItemType.ProfileBackground, backgrounds[1].Id, 120_000u, Currency.Coins),
                        new StoreItem(3u, $"[{backgrounds[2].Name}]({backgrounds[2].Url})",
                            StoreItemType.ProfileBackground, backgrounds[2].Id, 120_000u, Currency.Coins),
                        new StoreItem(4u, $"[{backgrounds[3].Name}]({backgrounds[3].Url})",
                            StoreItemType.ProfileBackground, backgrounds[3].Id, 120_000u, Currency.Coins),
                        new StoreItem(5u, $"[{backgrounds[4].Name}]({backgrounds[4].Url})",
                            StoreItemType.ProfileBackground, backgrounds[4].Id, 120_000u, Currency.Coins),
                        new StoreItem(6u, $"[{backgrounds[5].Name}]({backgrounds[5].Url})",
                            StoreItemType.ProfileBackground, backgrounds[5].Id, 120_000u, Currency.Coins),
                        new StoreItem(7u, $"[{backgrounds[6].Name}]({backgrounds[6].Url})",
                            StoreItemType.ProfileBackground, backgrounds[6].Id, 200_000u, Currency.Coins),
                        new StoreItem(8u, $"[{backgrounds[7].Name}]({backgrounds[7].Url})",
                            StoreItemType.ProfileBackground, backgrounds[7].Id, 200_000u, Currency.Coins),
                        new StoreItem(9u, $"[{backgrounds[8].Name}]({backgrounds[8].Url})",
                            StoreItemType.ProfileBackground, backgrounds[8].Id, 200_000u, Currency.Coins),
                        new StoreItem(10u, $"[{backgrounds[9].Name}]({backgrounds[9].Url})",
                            StoreItemType.ProfileBackground, backgrounds[9].Id, 400_000u, Currency.Coins),
                    };
                }

                return backgroundShopList;
            }
        }

        IonicMessage itemShopMessage;

        public IonicMessage ItemShopMessage
        {
            get
            {
                if (itemShopMessage is null)
                    itemShopMessage = Task.Run(
                        async () =>
                            await RiftBot.GetService<MessageService>()
                                .FormatMessageAsync(
                                    new RiftMessage
                                    {
                                        Embed = JsonConvert.SerializeObject(
                                            new RiftEmbed()
                                                .WithTitle("Магазин")
                                                .WithDescription(
                                                    "В магазине находятся сундуки, капсулы, бонусы и билеты.\n" +
                                                    "Для покупки напишите `!купить` и номер желаемого товара.")
                                                .AddField(
                                                    "Товар",
                                                    string.Join(
                                                        '\n',
                                                        ItemShopList.Select(
                                                            x =>
                                                                $"{x.Id.ToString()}. {x.FormattedName}")),
                                                    true)
                                                .AddField("Стоимость",
                                                    string.Join(
                                                        '\n',
                                                        ItemShopList.Select(
                                                            x => x
                                                                .FormattedPrice)),
                                                    true)
                                                .WithFooter(
                                                    "Максимум одна покупка в час."))
                                    })).Result;

                return itemShopMessage;
            }
        }

        IonicMessage roleShopMessage;

        public IonicMessage RoleShopMessage
        {
            get
            {
                if (roleShopMessage is null)
                    roleShopMessage = Task.Run(
                        async () =>
                            await RiftBot.GetService<MessageService>()
                                .FormatMessageAsync(
                                    new RiftMessage
                                    {
                                        Embed = JsonConvert.SerializeObject(
                                            new RiftEmbed()
                                                .WithTitle("Магазин ролей")
                                                .WithDescription(
                                                    "В магазине находятся обычные и уникальные роли.\n" +
                                                    "Для покупки напишите `!купить роль` и номер желаемого товара.")
                                                .AddField(
                                                    "Товар",
                                                    string.Join(
                                                        '\n',
                                                        RoleShopList.Select(
                                                            x =>
                                                                $"{x.Id.ToString()}. {x.FormattedName}")),
                                                    true)
                                                .AddField("Стоимость",
                                                    string.Join(
                                                        '\n',
                                                        RoleShopList.Select(
                                                            x => x
                                                                .FormattedPrice)),
                                                    true)
                                                .WithFooter(
                                                    "Максимум одна покупка в 24 часа."))
                                    })).Result;

                return roleShopMessage;
            }
        }

        IonicMessage backgroundShopMessage;

        public IonicMessage BackgroundShopMessage
        {
            get
            {
                if (backgroundShopMessage is null)
                {
                    var things = BackgroundShopList.Select(x => $"{x.Id.ToString()}. {x.FormattedName}");
                    var prices = BackgroundShopList.Select(x => x.FormattedPrice);
                    var embed = new RiftEmbed()
                        .WithTitle("Магазин фонов")
                        .WithDescription("В магазине находятся фоны для вашего профиля.\n" +
                                         "Для покупки напишите `!купить фон` и номер желаемого товара.")
                        .AddField("Товар", string.Join('\n', things), true)
                        .AddField("Стоимость", string.Join('\n', prices), true)
                        .WithFooter("Максимум одна покупка в час.");
                    var json = JsonConvert.SerializeObject(embed);
                    var msg = new RiftMessage {Embed = json};

                    backgroundShopMessage = Task.Run(async () =>
                            await RiftBot.GetService<MessageService>()
                                .FormatMessageAsync(msg))
                        .Result;
                }

                return backgroundShopMessage;
            }
        }

        StoreItem GetItemById(uint id)
        {
            return ItemShopList.FirstOrDefault(x => x.Id == id);
        }

        StoreItem GetRoleById(uint id)
        {
            return RoleShopList.FirstOrDefault(x => x.Id == id);
        }

        StoreItem GetBackgroundById(uint id)
        {
            return BackgroundShopList.FirstOrDefault(x => x.Id == id);
        }

        static async Task<bool> TryWithdrawAsync(ulong userId, StoreItem item)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            switch (item.Currency)
            {
                case Currency.Coins:
                {
                    if (dbInventory.Coins < item.Price)
                        return false;

                    await DB.Inventory.RemoveAsync(userId, new InventoryData {Coins = item.Price});
                    break;
                }

                case Currency.Tokens:
                {
                    if (dbInventory.Tokens < item.Price)
                        return false;

                    await DB.Inventory.RemoveAsync(userId, new InventoryData {Tokens = item.Price});
                    break;
                }
            }

            return true;
        }

        public async Task<IonicMessage> PurchaseItemAsync(ulong userId, uint itemId)
        {
            await MutexItemStore.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                result = await PurchaseItemInternalAsync(userId, itemId).ConfigureAwait(false);
            }
            finally
            {
                MutexItemStore.Release();
            }

            return result;
        }

        async Task<IonicMessage> PurchaseItemInternalAsync(ulong userId, uint itemId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var item = GetItemById(itemId);

            if (item is null)
                return await RiftBot.GetMessageAsync("store-wrongnumber", new FormatData(userId));

            if (!await CanBuyItemAsync(userId))
                return await RiftBot.GetMessageAsync("itemstore-cooldown", new FormatData(userId));

            ItemReward reward;

            switch (item.Type)
            {
                case StoreItemType.Chest:
                    reward = new ItemReward().AddChests(item.Amount);
                    break;

                case StoreItemType.BotRespect:
                    reward = new ItemReward().AddBotRespects(1u);
                    break;

                case StoreItemType.Sphere:
                    reward = new ItemReward().AddSpheres(1u);
                    break;

                default:
                    RiftBot.Log.Error($"Wrong type in item store: {item.Type.ToString()}!");
                    return MessageService.Error;
            }

            if (!await TryWithdrawAsync(userId, item))
                switch (item.Currency)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens:
                        return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }

            if (item.Type == StoreItemType.Chest)
                BoughtChests?.Invoke(null, new BoughtChestsEventArgs(userId, item.Amount));

            await reward.DeliverToAsync(userId);

            await DB.Cooldowns.SetLastItemStoreTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData {PurchasedItems = item.Amount});

            RiftBot.Log.Info($"Item purchased: #{item.Id.ToString()} by {userId.ToString()}.");

            return await RiftBot.GetMessageAsync("store-success", new FormatData(userId)
            {
                Reward = reward
            });
        }

        static async Task<bool> CanBuyItemAsync(ulong userId)
        {
            var dbStore = await DB.Cooldowns.GetAsync(userId);

            if (dbStore.LastItemStoreTime == DateTime.MinValue)
                return true;

            var diff = DateTime.UtcNow - dbStore.LastItemStoreTime;

            return diff > Settings.Economy.ItemStoreCooldown;
        }

        public async Task<IonicMessage> PurchaseRoleAsync(ulong userId, uint itemId)
        {
            await MutexRoleStore.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                result = await PurchaseRoleInternalAsync(userId, itemId).ConfigureAwait(false);
            }
            finally
            {
                MutexRoleStore.Release();
            }

            return result;
        }

        async Task<IonicMessage> PurchaseRoleInternalAsync(ulong userId, uint itemId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var item = GetRoleById(itemId);

            if (item is null)
                return await RiftBot.GetMessageAsync("store-wrongnumber", new FormatData(userId));

            if (!await CanBuyRoleAsync(userId))
                return await RiftBot.GetMessageAsync("rolestore-cooldown", new FormatData(userId));

            if (await DB.RoleInventory.HasAnyAsync(userId, item.DatabaseId))
                return await RiftBot.GetMessageAsync("rolestore-hasrole", new FormatData(userId));

            var role = await DB.Roles.GetAsync(item.DatabaseId);

            if (role is null)
                return MessageService.Error;

            if (!await TryWithdrawAsync(userId, item))
                switch (item.Currency)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens:
                        return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }

            RolesPurchased?.Invoke(null, new RolesPurchasedEventArgs(userId, 1u));

            await DB.RoleInventory.AddAsync(userId, role.Id, "Store");

            var reward = new RoleReward().SetRole(item.RoleId);

            await DB.Cooldowns.SetLastRoleStoreTimeAsync(userId, DateTime.UtcNow);

            RiftBot.Log.Info($"Role purchased: #{item.RoleId.ToString()} by {userId.ToString()}.");

            return await RiftBot.GetMessageAsync("store-success", new FormatData(userId)
            {
                Reward = reward
            });
        }

        static async Task<bool> CanBuyRoleAsync(ulong userId)
        {
            var dbStore = await DB.Cooldowns.GetAsync(userId);

            if (dbStore.LastRoleStoreTime == DateTime.MinValue)
                return true;

            var diff = DateTime.UtcNow - dbStore.LastRoleStoreTime;

            return diff > Settings.Economy.RoleStoreCooldown;
        }

        public async Task<IonicMessage> PurchaseBackgroundAsync(ulong userId, uint itemId)
        {
            await MutexBackStore.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                result = await PurchaseBackgroundInternalAsync(userId, itemId).ConfigureAwait(false);
            }
            finally
            {
                MutexBackStore.Release();
            }

            return result;
        }

        async Task<IonicMessage> PurchaseBackgroundInternalAsync(ulong userId, uint itemId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var item = GetBackgroundById(itemId);

            if (item is null)
                return await RiftBot.GetMessageAsync("store-wrongnumber", new FormatData(userId));

            if (!await CanBuyBackAsync(userId))
                return await RiftBot.GetMessageAsync("backstore-cooldown", new FormatData(userId));

            var backInventory = await DB.BackgroundInventory.GetAsync(userId);

            if (!(backInventory is null) && backInventory.Count > 0)
                if (backInventory.Any(x => x.BackgroundId == item.Id))
                    return await RiftBot.GetMessageAsync("backstore-hasback", new FormatData(userId));

            if (!await TryWithdrawAsync(userId, item))
                switch (item.Currency)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens:
                        return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }

            var reward = new BackgroundReward().SetId((int) item.Id);

            try
            {
                await reward.DeliverToAsync(userId);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return MessageService.Error;
            }

            await DB.Cooldowns.SetLastBackgroundStoreTimeAsync(userId, DateTime.UtcNow);

            RiftBot.Log.Info($"Background purchased: #{item.Id.ToString()} by {userId.ToString()}.");

            return await RiftBot.GetMessageAsync("store-success", new FormatData(userId)
            {
                Reward = reward
            });
        }

        static async Task<bool> CanBuyBackAsync(ulong userId)
        {
            var dbStore = await DB.Cooldowns.GetAsync(userId);

            if (dbStore.LastBackgroundStoreTime == DateTime.MinValue)
                return true;

            var diff = DateTime.UtcNow - dbStore.LastBackgroundStoreTime;

            return diff > Settings.Economy.BackgroundStoreCooldown;
        }
    }
}
