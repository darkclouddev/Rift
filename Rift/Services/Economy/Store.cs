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
            new StoreItem(1u, "$emotechest", "Сундук", StoreItemType.Chest, 600u, Currency.Coins),
            new StoreItem(2u, "$emotesphere", "Сфера", StoreItemType.Sphere, 25_000u, Currency.Coins),
            new StoreItem(3u, "$emotecapsule", "Капсула", StoreItemType.Capsule, 100u, Currency.Tokens),
        };

        static readonly List<StoreItem> StoreTickets = new List<StoreItem>
        {
            new StoreItem(4u, "$emoteticket", "Билет", StoreItemType.Ticket, 4_000u, Currency.Coins),
        };

        static readonly List<StoreItem> StoreTempRoles = new List<StoreItem>
        {
            
        };

        static readonly List<StoreItem> StorePermanentRoles = new List<StoreItem>
        {
            new StoreItem(9u, "$emoteroles", "Токсичные", StoreItemType.PermanentRole, Settings.RoleId.Toxic, 40u, Currency.Tokens),
            new StoreItem(10u, "$emoteroles", "Вардилочки", StoreItemType.PermanentRole, Settings.RoleId.Wardhole, 40u, Currency.Tokens),
            new StoreItem(11u, "$emoteroles", "Престижные", StoreItemType.PermanentRole, Settings.RoleId.Prestige, 50u, Currency.Tokens),
            new StoreItem(12u, "$emoteroles", "K/DA", StoreItemType.PermanentRole, Settings.RoleId.KDA, 80u, Currency.Tokens),
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
                    CurrencyEmote = "$emotecoins";
                    break;

                case Currency.Tokens:
                    CurrencyEmote = "$emotetokens";
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
                    CurrencyEmote = "$emotecoins";
                    break;

                case Currency.Tokens:
                    CurrencyEmote = "$emotetokens";
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
