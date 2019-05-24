using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;

namespace Rift.Services.Reward
{
    public class ItemReward : RewardBase
    {
        public uint Coins { get; private set; } = 0u;

        public uint Tokens { get; private set; } = 0u;

        public uint Chests { get; private set; } = 0u;

        public uint Spheres { get; private set; } = 0u;

        public uint Capsules { get; private set; } = 0u;

        public uint Tickets { get; private set; } = 0u;

        public uint DoubleExps { get; private set; } = 0u;

        public uint BotRespects { get; private set; } = 0u;

        public uint Rewinds { get; private set; } = 0u;

        public ItemReward()
        {
            Type = RewardType.Item;
        }

        public ItemReward AddCoins(uint amount)
        {
            Coins += amount;
            return this;
        }

        public ItemReward AddTokens(uint amount)
        {
            Tokens += amount;
            return this;
        }

        public ItemReward AddChests(uint amount)
        {
            Chests += amount;
            return this;
        }

        public ItemReward AddSpheres(uint amount)
        {
            Spheres += amount;
            return this;
        }

        public ItemReward AddCapsules(uint amount)
        {
            Capsules += amount;
            return this;
        }

        public ItemReward AddTickets(uint amount)
        {
            Capsules += amount;
            return this;
        }

        public ItemReward AddDoubleExps(uint amount)
        {
            DoubleExps += amount;
            return this;
        }

        public ItemReward AddBotRespects(uint amount)
        {
            BotRespects += amount;
            return this;
        }

        public ItemReward AddRewinds(uint amount)
        {
            Rewinds += amount;
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            await Database.AddInventoryAsync(userId, new InventoryData
            {
                Coins = Coins,
                Tokens = Tokens,
                Chests = Chests,
                Spheres = Spheres,
                Capsules = Capsules,
                Tickets = Tickets,
                DoubleExps = DoubleExps,
                BotRespects = BotRespects,
                Rewinds = Rewinds,
            });
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Coins > 0u) sb.Append($"{Settings.Emote.Coin} {Coins.ToString()} ");
            if (Tokens > 0u) sb.Append($"{Settings.Emote.Token} {Tokens.ToString()} ");
            if (Chests > 0u) sb.Append($"{Settings.Emote.Chest} {Chests.ToString()} ");
            if (Spheres > 0u) sb.Append($"{Settings.Emote.Sphere} {Spheres.ToString()} ");
            if (Capsules > 0u) sb.Append($"{Settings.Emote.Capsule} {Capsules.ToString()} ");
            if (Tickets > 0u) sb.Append($"{Settings.Emote.Tickets} {Tickets.ToString()} ");
            if (DoubleExps > 0u) sb.Append($"{Settings.Emote.PowerupDoubleExperience} {DoubleExps.ToString()} ");
            if (BotRespects > 0u) sb.Append($"{Settings.Emote.BotRespect} {BotRespects.ToString()} ");
            if (Rewinds > 0u) sb.Append($"{Settings.Emote.Yasuo} {Rewinds.ToString()} ");

            return sb.ToString().TrimEnd();
        }
    }
}
