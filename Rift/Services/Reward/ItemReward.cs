using System.Text;
using System.Threading.Tasks;

using IonicLib.Extensions;
using Rift.Database;

namespace Rift.Services.Reward
{
    public class ItemReward : RewardBase
    {
        public uint Coins { get; set; } = 0u;

        public uint Tokens { get; set; } = 0u;

        public uint Chests { get; set; } = 0u;

        public uint Spheres { get; set; } = 0u;

        public uint Capsules { get; set; } = 0u;

        public uint Tickets { get; set; } = 0u;

        public uint DoubleExps { get; set; } = 0u;

        public uint BotRespects { get; set; } = 0u;

        public uint Rewinds { get; set; } = 0u;

        public ItemReward()
        {
            Type = RewardType.Item;
        }

        public ItemReward AddCoins(uint amount)
        {
            Coins += amount;
            return this;
        }

        public ItemReward AddRandomCoins(int min, int max)
        {
            Coins += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddTokens(uint amount)
        {
            Tokens += amount;
            return this;
        }

        public ItemReward AddRandomTokens(int min, int max)
        {
            Tokens += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddChests(uint amount)
        {
            Chests += amount;
            return this;
        }

        public ItemReward AddRandomChests(int min, int max)
        {
            Chests += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddSpheres(uint amount)
        {
            Spheres += amount;
            return this;
        }

        public ItemReward AddRandomSpheres(int min, int max)
        {
            Spheres += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddCapsules(uint amount)
        {
            Capsules += amount;
            return this;
        }

        public ItemReward AddRandomCapsules(int min, int max)
        {
            Capsules += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddTickets(uint amount)
        {
            Capsules += amount;
            return this;
        }

        public ItemReward AddRandomTickets(int min, int max)
        {
            Capsules += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddDoubleExps(uint amount)
        {
            DoubleExps += amount;
            return this;
        }

        public ItemReward AddRandomDoubleExps(int min, int max)
        {
            DoubleExps += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddBotRespects(uint amount)
        {
            BotRespects += amount;
            return this;
        }

        public ItemReward AddRandomBotRespects(int min, int max)
        {
            BotRespects += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddRewinds(uint amount)
        {
            Rewinds += amount;
            return this;
        }

        public ItemReward AddRandomRewinds(int min, int max)
        {
            Rewinds += Helper.NextUInt(min, max);
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            await DB.Inventory.AddAsync(userId, new InventoryData
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
            var emotes = RiftBot.GetService<EmoteService>();
            var sb = new StringBuilder();
            if (Coins > 0u) sb.Append($"{emotes.GetEmoteString("$emotecoins")} {Coins.ToString()} ");
            if (Tokens > 0u) sb.Append($"{emotes.GetEmoteString("$emotetokens")} {Tokens.ToString()} ");
            if (Chests > 0u) sb.Append($"{emotes.GetEmoteString("$emotechest")} {Chests.ToString()} ");
            if (Spheres > 0u) sb.Append($"{emotes.GetEmoteString("$emotesphere")} {Spheres.ToString()} ");
            if (Capsules > 0u) sb.Append($"{emotes.GetEmoteString("$emotecapsule")} {Capsules.ToString()} ");
            if (Tickets > 0u) sb.Append($"{emotes.GetEmoteString("$emoteticket")} {Tickets.ToString()} ");
            if (DoubleExps > 0u) sb.Append($"{emotes.GetEmoteString("$emote2exp")} {DoubleExps.ToString()} ");
            if (BotRespects > 0u) sb.Append($"{emotes.GetEmoteString("$emoterespect")} {BotRespects.ToString()} ");
            if (Rewinds > 0u) sb.Append($"{emotes.GetEmoteString("$emoterewind")} {Rewinds.ToString()} ");

            return sb.ToString().TrimEnd();
        }

        public string ToPlainString()
        {
            var sb = new StringBuilder();
            if (Coins > 0u) sb.Append($"{nameof(Coins)} {Coins.ToString()} ");
            if (Tokens > 0u) sb.Append($"{nameof(Tokens)} {Tokens.ToString()} ");
            if (Chests > 0u) sb.Append($"{nameof(Chests)} {Chests.ToString()} ");
            if (Spheres > 0u) sb.Append($"{nameof(Spheres)} {Spheres.ToString()} ");
            if (Capsules > 0u) sb.Append($"{nameof(Capsules)} {Capsules.ToString()} ");
            if (Tickets > 0u) sb.Append($"{nameof(Tickets)} {Tickets.ToString()} ");
            if (DoubleExps > 0u) sb.Append($"{nameof(DoubleExps)} {DoubleExps.ToString()} ");
            if (BotRespects > 0u) sb.Append($"{nameof(BotRespects)} {BotRespects.ToString()} ");
            if (Rewinds > 0u) sb.Append($"{nameof(Rewinds)} {Rewinds.ToString()} ");

            return sb.ToString().TrimEnd();
        }
    }
}
