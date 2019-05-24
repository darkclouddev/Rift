using System.Collections.Generic;
using System.Linq;

using Rift.Configuration;

using Discord;

namespace Rift.Services.Economy
{
    public class Store
    {
        static Embed storeEmbed = null;

        public static Embed Embed
        {
            get
            {
                if (storeEmbed is null)
                {
                    storeEmbed = new EmbedBuilder()
                        .WithAuthor("Магазин")
                        .WithDescription($"В магазине находятся сундуки, билеты и роли.\n"
                                         + $"Для покупки напишите в чат `!купить` и номер желаемого товара.")
                        .AddField($"Основные товары{Settings.Emote.Invisible}{Settings.Emote.Invisible}{Settings.Emote.Invisible}{Settings.Emote.Invisible}",
                            string.Join("\n", StoreItems.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")),true)
                        
                        .AddField($"Стоимость{Settings.Emote.Invisible}",
                            string.Join("\n", StoreItems.Select(x => x.FormattedPrice)), true)
                        
                        .AddField($"Билеты для розыгрышей{Settings.Emote.Invisible} {Settings.Emote.Invisible}",
                            string.Join("\n", StoreTickets.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")),true)
                        
                        .AddField($"Стоимость{Settings.Emote.Invisible}",
                            string.Join("\n", StoreTickets.Select(x => x.FormattedPrice)), true)
                        
                        .AddField($"Роли на 30 дней {Settings.Emote.Invisible} {Settings.Emote.Invisible} {Settings.Emote.Invisible}{Settings.Emote.Invisible}",
                            string.Join("\n", StoreTempRoles.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")),true)
                        
                        .AddField($"Стоимость{Settings.Emote.Invisible}",
                            string.Join("\n", StoreTempRoles.Select(x => x.FormattedPrice)), true)
                        
                        .AddField($"Роли навсегда {Settings.Emote.Invisible} {Settings.Emote.Invisible} {Settings.Emote.Invisible}{Settings.Emote.Invisible}",
                            string.Join("\n", StorePermanentRoles.Select(x => $"{x.Id.ToString()}. {x.FormattedName}")), true)
                        
                        .AddField($"Стоимость{Settings.Emote.Invisible}",
                            string.Join("\n", StorePermanentRoles.Select(x => x.FormattedPrice)), true)
                        
                        .WithFooter($"Максимум одна покупка в 30 минут.")
                        .Build();
                }

                return storeEmbed;
            }
        }

        public static StoreItem GetShopItemById(uint id)
        {
            var item = StoreItems.FirstOrDefault(x => x.Id == id);

            if (!(item is null))
                return item;

            item = StoreTickets.FirstOrDefault(x => x.Id == id);

            if (!(item is null))
                return item;

            item = StoreTempRoles.FirstOrDefault(x => x.Id == id);

            if (!(item is null))
                return item;

            return StorePermanentRoles.FirstOrDefault(x => x.Id == id);
        }

        static readonly List<StoreItem> StoreItems = new List<StoreItem>
        {
            new StoreItem(1u, Settings.Emote.Chest, "Сундук", StoreItemType.Chest, 600u, Currency.Coins),
            new StoreItem(2u, Settings.Emote.Sphere, "Сфера", StoreItemType.Sphere, 25_000u, Currency.Coins),
            new StoreItem(3u, Settings.Emote.Capsule, "Капсула", StoreItemType.Capsule, 100u, Currency.Tokens),
        };

        static readonly List<StoreItem> StoreTickets = new List<StoreItem>
        {
            new StoreItem(4u, Settings.Emote.Tickets, "Билет", StoreItemType.Ticket, 4_000u, Currency.Coins),
        };

        static readonly List<StoreItem> StoreTempRoles = new List<StoreItem>
        {
            new StoreItem(6u, Settings.Emote.Chosen, "Избранные", StoreItemType.TempRole, Settings.RoleId.Chosen, 30_000u, Currency.Coins),
            new StoreItem(7u, Settings.Emote.DarkStar, "Темная звезда", StoreItemType.TempRole, Settings.RoleId.DarkStar, 35_000u, Currency.Coins),
            new StoreItem(8u, Settings.Emote.Mythic, "Мифические", StoreItemType.TempRole, Settings.RoleId.Mythic, 50_000u, Currency.Coins),
        };

        static readonly List<StoreItem> StorePermanentRoles = new List<StoreItem>
        {
            new StoreItem(9u, Settings.Emote.Roles, "Токсичные", StoreItemType.PermanentRole, Settings.RoleId.Toxic, 40u, Currency.Tokens),
            new StoreItem(10u, Settings.Emote.Roles, "Вардилочки", StoreItemType.PermanentRole, Settings.RoleId.Wardhole, 40u, Currency.Tokens),
            new StoreItem(11u, Settings.Emote.Roles, "Престижные", StoreItemType.PermanentRole, Settings.RoleId.Prestige, 50u, Currency.Tokens),
            new StoreItem(12u, Settings.Emote.Roles, "K/DA", StoreItemType.PermanentRole, Settings.RoleId.KDA, 80u, Currency.Tokens),
        };
    }

    public class StoreItem
    {
        public readonly uint Id;
        public readonly ulong RoleId;
        public readonly string Emote;
        public readonly string Name;
        public readonly StoreItemType Type;
        public readonly uint Price;
        public readonly Currency Currency;
        public readonly string CurrencyEmote;

        public string FormattedName => $"{Emote} {Name}";
        public string FormattedPrice => $"{CurrencyEmote} {Price.ToString()}";

        public StoreItem(uint id, string emote, string name, StoreItemType type, uint price, Currency currency)
        {
            Id = id;
            Emote = emote;
            Name = name;
            Type = type;
            Price = price;
            Currency = currency;

            switch (Currency)
            {
                case Currency.Coins:
                    CurrencyEmote = Settings.Emote.Coin;
                    break;

                case Currency.Tokens:
                    CurrencyEmote = Settings.Emote.Token;
                    break;
            }
        }

        public StoreItem(uint id, string emote, string name, StoreItemType type, ulong roleId, uint price, Currency currency)
        {
            Id = id;
            Emote = emote;
            Name = name;
            Type = type;
            RoleId = roleId;
            Price = price;
            Currency = currency;

            switch (Currency)
            {
                case Currency.Coins:
                    CurrencyEmote = Settings.Emote.Coin;
                    break;

                case Currency.Tokens:
                    CurrencyEmote = Settings.Emote.Token;
                    break;
            }
        }
    }

    public enum StoreItemType
    {
        Chest,
        Capsule,
        Token,
        Sphere,
        DoubleExp,
        BotRespect,
        Ticket,
        TempRole,
        PermanentRole,
    }
}
