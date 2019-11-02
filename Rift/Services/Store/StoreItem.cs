using System.Globalization;
using System.Threading.Tasks;

using Rift.Configuration;

namespace Rift.Services.Store
{
    public class StoreItem
    {
        public readonly uint Id;
        public readonly int DatabaseId;
        public readonly ulong RoleId;
        public readonly int BackgroundId;
        public readonly string Name;
        public readonly StoreItemType Type;
        public readonly uint Price;
        public readonly Currency Currency;
        public readonly string CurrencyEmote;
        public readonly uint Amount;

        public StoreItem(uint id, string name, StoreItemType type, uint amount, uint price, Currency currency)
        {
            Id = id;
            Name = name;
            Type = type;
            Amount = amount;
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

        public StoreItem(uint id, string name, StoreItemType type, int dbId, uint price, Currency currency)
        {
            Id = id;
            Name = name;
            Type = type;
            DatabaseId = dbId;
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

        public StoreItem(uint id, StoreItemType type, int dbId, uint price, Currency currency)
        {
            Id = id;
            Type = type;
            DatabaseId = dbId;
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

            switch (type)
            {
                case StoreItemType.PermanentRole:
                    var dbData = Task.Run(() => DB.Roles.GetAsync(dbId)).Result;
                    if (dbData is null)
                    {
                        RiftBot.Log.Error($"No data for role {dbId.ToString()}!");
                        break;
                    }

                    Name = dbData.Name;
                    RoleId = dbData.RoleId;
                    break;
            }
        }
    }

    public enum StoreItemType
    {
        Chest,
        BotRespect,
        Sphere,
        PermanentRole,
        ProfileBackground,
    }
}
