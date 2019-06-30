namespace Rift.Services.Store
{
    public class StoreItem
    {
        public readonly uint Id;
        public readonly ulong RoleId;
        public readonly string Name;
        public readonly StoreItemType Type;
        public readonly uint Price;
        public readonly Currency Currency;
        public readonly string CurrencyEmote;
        public readonly uint Amount;

        public string FormattedName
        {
            get
            {
                var emotes = RiftBot.GetService<EmoteService>();
                return $"{Name}{emotes.GetEmoteString("$emotetran")}";
            }
        }

        public string FormattedPrice
        {
            get
            {
                var emotes = RiftBot.GetService<EmoteService>();
                return $"{emotes.GetEmoteString(CurrencyEmote)} {Price.ToString()}";
            }
        }

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

        public StoreItem(uint id, string name, StoreItemType type, ulong roleId, uint price, Currency currency)
        {
            Id = id;
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

        public override string ToString()
        {
            return $"{Id.ToString()}. {Name} ({FormattedPrice})";
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
