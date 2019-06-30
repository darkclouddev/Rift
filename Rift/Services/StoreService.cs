using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;
using Rift.Data.Models;
using Rift.Database;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Services.Store;

using IonicLib;
using Newtonsoft.Json;

namespace Rift.Services
{
    public class StoreService
    {
        static readonly SemaphoreSlim MutexItemStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexRoleStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexBackStore = new SemaphoreSlim(1);

        List<StoreItem> itemShopList;
        List<StoreItem> ItemShopList
        {
            get
            {
                if (itemShopList is null)
                {
                    itemShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, "Сундуки (1 шт.)", StoreItemType.Chest, 1u, 2_000u, Currency.Coins),
                        new StoreItem(2u, "Сундуки (6 шт.)", StoreItemType.Chest, 6u, 11_000u, Currency.Coins),
                        new StoreItem(3u, "Уважение ботов", StoreItemType.BotRespect, 1u, 24_000u, Currency.Coins),
                        new StoreItem(4u, "Сфера", StoreItemType.Sphere, 1u, 40_000u, Currency.Coins),
                    };
                }

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
                    roleShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, "Мародеры", StoreItemType.PermanentRole, Settings.RoleId.Marauders, 50_000u, Currency.Coins),
                        new StoreItem(2u, "Аркадные", StoreItemType.PermanentRole, Settings.RoleId.Arcade, 200_000u, Currency.Coins),
                        new StoreItem(3u, "Галантные", StoreItemType.PermanentRole, Settings.RoleId.Debonairs, 200_000u, Currency.Coins),
                        new StoreItem(4u, "Хрустальные", StoreItemType.PermanentRole, Settings.RoleId.Crystal, 300_000u, Currency.Coins),
                        new StoreItem(5u, "Тусовые", StoreItemType.PermanentRole, Settings.RoleId.Party, 400_000u, Currency.Coins),
                        new StoreItem(6u, "Вардилочки", StoreItemType.PermanentRole, Settings.RoleId.Wardhole, 600_000u, Currency.Coins),
                        new StoreItem(7u, "Темная звезда", StoreItemType.PermanentRole, Settings.RoleId.DarkStar, 700_000u, Currency.Coins),
                        new StoreItem(8u, "Инфернальные", StoreItemType.PermanentRole, Settings.RoleId.Infernal, 750_000u, Currency.Coins),
                        new StoreItem(9u, "Звездные защитники", StoreItemType.PermanentRole, Settings.RoleId.StarGuardians, 800_000u, Currency.Coins),
                        new StoreItem(10u, "Вознесение", StoreItemType.PermanentRole, Settings.RoleId.Ascention, 250u, Currency.Tokens),
                        new StoreItem(11u, "Импульсные", StoreItemType.PermanentRole, Settings.RoleId.Impulse, 250u, Currency.Tokens),
                        new StoreItem(12u, "Кровавая луна", StoreItemType.PermanentRole, Settings.RoleId.BloodMoon, 300u, Currency.Tokens),
                        new StoreItem(13u, "Юстициары", StoreItemType.PermanentRole, Settings.RoleId.Justicars, 350u, Currency.Tokens),
                        new StoreItem(14u, "Токсичные", StoreItemType.PermanentRole, Settings.RoleId.Toxic, 400u, Currency.Tokens),
                        new StoreItem(15u, "K/DA", StoreItemType.PermanentRole, Settings.RoleId.KDA, 500u, Currency.Tokens),
                    };
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
                    const string url = "https://cdn.ionpri.me/rift/profile/backgrounds/{0}.jpg";

                    backgroundShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, $"[Preview]({string.Format(url, 1.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 70_000u, Currency.Coins),
                        new StoreItem(2u, $"[Preview]({string.Format(url, 2.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 120_000u, Currency.Coins),
                        new StoreItem(3u, $"[Preview]({string.Format(url, 3.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 120_000u, Currency.Coins),
                        new StoreItem(4u, $"[Preview]({string.Format(url, 4.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 120_000u, Currency.Coins),
                        new StoreItem(5u, $"[Preview]({string.Format(url, 5.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 120_000u, Currency.Coins),
                        new StoreItem(6u, $"[Preview]({string.Format(url, 6.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 120_000u, Currency.Coins),
                        new StoreItem(7u, $"[Preview]({string.Format(url, 7.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 200_000u, Currency.Coins),
                        new StoreItem(8u, $"[Preview]({string.Format(url, 8.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 200_000u, Currency.Coins),
                        new StoreItem(9u, $"[Preview]({string.Format(url, 9.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 200_000u, Currency.Coins),
                        new StoreItem(10u, $"[Preview]({string.Format(url, 10.ToString())})",
                            StoreItemType.ProfileBackground, 1u, 400_000u, Currency.Coins),
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
                {
                    itemShopMessage = Task.Run(async () =>
                        await RiftBot.GetService<MessageService>()
                            .FormatMessageAsync(
                                new RiftMessage
                                {
                                    Embed = JsonConvert.SerializeObject(new RiftEmbed()
                                        .WithTitle("Магазин")
                                        .WithDescription("В магазине находятся сундуки, капсулы, бонусы и билеты.\n" +
                                                         "Для покупки напишите `!купить` и номер желаемого товара.")
                                        .AddField("Товар", string.Join('\n', ItemShopList.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")), true)
                                        .AddField("Стоимость", string.Join('\n', ItemShopList.Select(x => x.FormattedPrice)), true)
                                        .WithFooter("Максимум одна покупка в час."))
                                })).Result;
                }

                return itemShopMessage;
            }
        }

        IonicMessage roleShopMessage;
        public IonicMessage RoleShopMessage
        {
            get
            {
                if (roleShopMessage is null)
                {
                    roleShopMessage = Task.Run(async () =>
                        await RiftBot.GetService<MessageService>()
                            .FormatMessageAsync(
                                new RiftMessage
                                {
                                    Embed = JsonConvert.SerializeObject(new RiftEmbed()
                                        .WithTitle("Магазин ролей")
                                        .WithDescription("В магазине находятся обычные и уникальные роли.\n" +
                                                         "Для покупки напишите `!купить роль` и номер желаемого товара.")
                                        .AddField("Товар", string.Join('\n', RoleShopList.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")), true)
                                        .AddField("Стоимость", string.Join('\n', RoleShopList.Select(x => x.FormattedPrice)), true)
                                        .WithFooter("Максимум одна покупка в 24 часа."))
                                })).Result;
                }

                return roleShopMessage;
            }
        }
        
        public IonicMessage BackgroundShopMessage { get; }

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
            return backgroundShopList.FirstOrDefault(x => x.Id == id);
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

                    await DB.Inventory.RemoveAsync(userId, new InventoryData { Coins = item.Price });
                    break;
                }

                case Currency.Tokens:
                {
                    if (dbInventory.Tokens < item.Price)
                        return false;

                    await DB.Inventory.RemoveAsync(userId, new InventoryData { Tokens = item.Price });
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
            {
                switch (item.Currency)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens: return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }
            }

            await reward.DeliverToAsync(userId);

            await DB.Cooldowns.SetLastItemStoreTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData { PurchasedItems = item.Amount });

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

            var roleInventory = await DB.RoleInventory.GetAsync(userId);

            if (!(roleInventory is null) && roleInventory.Count > 0)
            {
                if (roleInventory.Any(x => x.RoleId == item.RoleId))
                    return await RiftBot.GetMessageAsync("rolestore-hasrole", new FormatData(userId));
            }
            
            if (!await TryWithdrawAsync(userId, item))
            {
                switch (item.Currency)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens: return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }
            }
            
            await DB.RoleInventory.AddAsync(userId, item.RoleId, "Store");

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
    }
}
