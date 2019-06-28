using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Database;
using Settings = Rift.Configuration.Settings;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Services.Store;

using Newtonsoft.Json;
using IonicLib;

namespace Rift.Services
{
    public class StoreService
    {
        static readonly SemaphoreSlim MutexItemStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexRoleStore = new SemaphoreSlim(1);
        static readonly SemaphoreSlim MutexBackStore = new SemaphoreSlim(1);

        List<StoreItem> _itemShopList;

        List<StoreItem> itemShopList
        {
            get
            {
                if (_itemShopList is null)
                {
                    _itemShopList = new List<StoreItem>
                    {
                        new StoreItem(1u, "Сундуки (1 шт.)", StoreItemType.Chest, 1u, 2_000u, Currency.Coins),
                        new StoreItem(2u, "Сундуки (6 шт.)", StoreItemType.Chest, 6u, 11_000u, Currency.Coins),
                        new StoreItem(3u, "Уважение ботов", StoreItemType.BotRespect, 1u, 24_000u, Currency.Coins),
                        new StoreItem(4u, "Сфера", StoreItemType.Sphere, 1u, 40_000u, Currency.Coins),
                    };
                }

                return _itemShopList;
            }
        }

        List<StoreItem> roleShopList;
        List<StoreItem> backgroundShopList;

        IonicMessage _itemShopMessage;
        public IonicMessage ItemShopMessage
        {
            get
            {
                if (_itemShopMessage is null)
                {
                    _itemShopMessage = Task.Run(async () =>
                        await RiftBot.GetService<MessageService>()
                            .FormatMessageAsync(
                                new RiftMessage
                                {
                                    Text = "**Магазин**\nВ магазине находятся сундуки, капсулы, бонусы и билеты.\n" +
                                           "Для покупки напишите `!купить` и номер желаемого товара.",
                                    Embed = JsonConvert.SerializeObject(new RiftEmbed()
                                        .AddField("Товар", string.Join('\n', itemShopList.Select(x => $"{x.Id}. {x.FormattedName}")), true)
                                        .AddField("Стоимость", string.Join('\n', itemShopList.Select(x => x.FormattedPrice)), true)
                                        .WithFooter("Максимум одна покупка в час."))
                                })).Result;
                }

                return _itemShopMessage;
            }
        }
        public IonicMessage RoleShopMessage { get; }
        public IonicMessage BackgroundShopMessage { get; }

        public StoreService()
        {
            //roleShopList = new List<StoreItem>
            //{
            //    new StoreItem(1u, "Мародеры", StoreItemType.PermanentRole, Settings.RoleId.Marauders, 50_000u, Currency.Coins),
            //    new StoreItem(2u, "Аркадные", StoreItemType.PermanentRole, Settings.RoleId.Arcade, 200_000u, Currency.Coins),
            //    new StoreItem(3u, "Галантные", StoreItemType.PermanentRole, Settings.RoleId.Debonairs, 200_000u, Currency.Coins),
            //    new StoreItem(4u, "Хрустальные", StoreItemType.PermanentRole, Settings.RoleId.Crystal, 300_000u, Currency.Coins),
            //    new StoreItem(5u, "Тусовые", StoreItemType.PermanentRole, Settings.RoleId.Party, 400_000u, Currency.Coins),
            //    new StoreItem(6u, "Вардилочки", StoreItemType.PermanentRole, Settings.RoleId.Wardhole, 600_000u, Currency.Coins),
            //    new StoreItem(7u, "Темная звезда", StoreItemType.PermanentRole, Settings.RoleId.DarkStar, 700_000u, Currency.Coins),
            //    new StoreItem(8u, "Инфернальные", StoreItemType.PermanentRole, Settings.RoleId.Infernal, 750_000u, Currency.Coins),
            //    new StoreItem(9u, "Звездные защитники", StoreItemType.PermanentRole, Settings.RoleId.StarGuardians, 50_000u, Currency.Coins),
            //    new StoreItem(10u, "Вознесение", StoreItemType.PermanentRole, Settings.RoleId.Ascention, 250u, Currency.Tokens),
            //    new StoreItem(11u, "Импульсные", StoreItemType.PermanentRole, Settings.RoleId.Impulse, 250u, Currency.Tokens),
            //    new StoreItem(12u, "Кровавая луна", StoreItemType.PermanentRole, Settings.RoleId.BloodMoon, 300u, Currency.Tokens),
            //    new StoreItem(13u, "Юстициары", StoreItemType.PermanentRole, Settings.RoleId.Justicars, 350u, Currency.Tokens),
            //    new StoreItem(14u, "Токсичные", StoreItemType.PermanentRole, Settings.RoleId.Toxic, 400u, Currency.Tokens),
            //    new StoreItem(15u, "K/DA", StoreItemType.PermanentRole, Settings.RoleId.KDA, 500u, Currency.Tokens),
            //};

            //var rolesList = string.Join('\n', roleShopList.Select(x => x.ToString()));

            //RoleShopMessage = new IonicMessage(
            //    new RiftEmbed()
            //        .WithTitle("Магазин ролей")
            //        .WithDescription("В магазине находятся обычные и уникальные роли.\n" +
            //                         "Для покупки напишите `!купить роль` и номер желаемого товара.\n\n" +
            //                         rolesList)
            //    );

            //backgroundShopList = new List<StoreItem>
            //{

            //};

            //var backgroundsList = string.Join('\n', backgroundShopList.Select(x => x.ToString()));
        }

        StoreItem GetItemById(uint id)
        {
            return itemShopList.FirstOrDefault(x => x.Id == id);
        }

        StoreItem GetRoleById(uint id)
        {
            return roleShopList.FirstOrDefault(x => x.Id == id);
        }

        StoreItem GetBackgroundById(uint id)
        {
            return backgroundShopList.FirstOrDefault(x => x.Id == id);
        }

        static readonly IonicMessage wrongNumberMessage = new IonicMessage("Неверный номер предмета!");

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

        async Task<IonicMessage> PurchaseItemInternalAsync(ulong userId, uint id)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var item = GetItemById(id);

            if (item is null)
                return wrongNumberMessage;

            if (!await CanBuyItemAsync(userId)) // TODO: remove admin check after tests
                return await RiftBot.GetMessageAsync("itemstore-cooldown", new FormatData(userId));

            (var result, var currencyType) = await TryWithdrawAsync(userId, item);

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens: return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }
            }

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

            await reward.DeliverToAsync(userId);

            await DB.Cooldowns.SetLastItemStoreTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData { PurchasedItems = item.Amount });

            RiftBot.Log.Info($"Item purchased: #{item.Id.ToString()} by {userId.ToString()}.");

            return await RiftBot.GetMessageAsync("store-success", new FormatData(userId)
            {
                Reward = reward
            });
        }

        async Task StorePurchaseInternalAsync(ulong userId, StoreItem item)
        {
            // if buying temp role over existing one
            if (item.Type == StoreItemType.PermanentRole)
            {
                // TODO: check role inventory

                var userTempRoles = await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId);

                if (userTempRoles != null && userTempRoles.Count > 0)
                {
                    if (userTempRoles.Any(x => x.UserId == userId && x.RoleId == item.RoleId))
                    {
                        await RiftBot.GetMessageAsync("store-hasrole", new FormatData(userId));
                    }
                }
            }

            switch (item.Type)
            {
                case StoreItemType.PermanentRole:
                    var msgPermRole = await RiftBot.GetMessageAsync("store-purchased-permrole", new FormatData(userId));
                    //await channel.SendIonicMessageAsync(msgPermRole);
                    // TODO: add to role inventory
                    await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, item.RoleId);
                    break;
            }
        }

        static async Task<bool> CanBuyItemAsync(ulong userId)
        {
            var dbStore = await DB.Cooldowns.GetAsync(userId);

            if (dbStore.LastItemStoreTime == DateTime.MinValue)
                return true;

            var diff = DateTime.UtcNow - dbStore.LastItemStoreTime;

            return diff > Settings.Economy.StoreCooldown;
        }

        async Task<(bool, Currency)> TryWithdrawAsync(ulong userId, StoreItem item)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            switch (item.Currency)
            {
                case Currency.Coins:
                {
                    if (dbInventory.Coins < item.Price)
                        return (false, item.Currency);

                    await DB.Inventory.RemoveAsync(userId, new InventoryData { Coins = item.Price });
                    break;
                }

                case Currency.Tokens:
                {
                    if (dbInventory.Tokens < item.Price)
                        return (false, item.Currency);

                    await DB.Inventory.RemoveAsync(userId, new InventoryData { Tokens = item.Price });
                    break;
                }
            }

            return (true, item.Currency);
        }
    }
}
