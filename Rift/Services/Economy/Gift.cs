using System;
using System.Collections.Generic;
using System.Linq;

using Rift.Configuration;

using Discord;

namespace Rift.Services.Economy
{
    public class Gift
    {
        static Embed giftEmbed = null;

        public static Embed Embed
        {
            get
            {
                if (giftEmbed is null)
                {
                    giftEmbed = new EmbedBuilder()
                                .WithAuthor("Подарки")
                                .WithDescription($"Радуйте других призывателей.\nДля отправки подарка другому призывателю используйте\nкоманду `!подарить` с указанием номера желаемого подарка")
                                .AddField("Подарки",
                                          String.Join("\n", giftItems.Select(x => $"{x.Id}. {x.FormattedName}")), true)
                                .AddField("Стоимость", String.Join("\n", giftItems.Select(x => x.FormattedPrice)), true)
                                .WithFooter($"Максимум один подарок в час.")
                                .Build();
                }

                return giftEmbed;
            }
        }

        public static GiftItem GetGiftItemById(uint id) => giftItems.FirstOrDefault(x => x.Id == id);

        static readonly List<GiftItem> giftItems = new List<GiftItem>
        {
            new GiftItem(1u, Settings.Emote.Chest, "Сундук", GiftItemType.Chest, 800u,
                         Currency.Coins),
            new GiftItem(2u, Settings.Emote.Coin, "Монеты (рандом)", GiftItemType.CoinsRandom, 3_000u,
                         Currency.Coins),
            new GiftItem(3u, Settings.Emote.UsualTickets, "Обычный билет", GiftItemType.UsualTicket, 4_000u,
                         Currency.Coins),
            new GiftItem(4u, Settings.Emote.RareTickets, "Редкий билет", GiftItemType.RareTicket, 20_000u,
                         Currency.Coins),
        };
    }

    public class GiftItem
    {
        public readonly uint Id;
        public readonly string Emote;
        public readonly string Name;
        public readonly GiftItemType Type;
        public readonly uint Price;
        public readonly Currency Currency;
        public readonly string CurrencyEmote;

        public string FormattedName => $"{Emote} {Name}";
        public string FormattedPrice => $"{CurrencyEmote} {Price}";

        public GiftItem(uint id, string emote, string name, GiftItemType type, uint price,
                        Currency currency)
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
    }

    public enum GiftItemType
    {
        CoinsRandom,
        ChestsRandom,
        Chest,
        Sphere,
        UsualTicket,
        BotRespect,
        RareTicket,
        DoubleExp,
    }
}
