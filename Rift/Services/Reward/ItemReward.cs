using System.Text;
using System.Threading.Tasks;

using Rift.Database;

using IonicLib.Extensions;

namespace Rift.Services.Reward
{
    public class ItemReward : RewardBase
    {
        public uint? Coins { get; set; }

        public uint? Tokens { get; set; }

        public uint? Chests { get; set; }

        public uint? Spheres { get; set; }

        public uint? Capsules { get; set; }

        public uint? Tickets { get; set; }

        public uint? DoubleExps { get; set; }

        public uint? BotRespects { get; set; }

        public uint? Rewinds { get; set; }

        public ItemReward()
        {
            Type = RewardType.Item;
        }

        public ItemReward AddCoins(uint amount)
        {
            if (!Coins.HasValue)
                Coins = amount;
            else
                Coins += amount;

            return this;
        }

        public ItemReward AddRandomCoins(int min, int max)
        {
            if (!Coins.HasValue)
                Coins = 0u;

            Coins += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddTokens(uint amount)
        {
            if (!Tokens.HasValue)
                Tokens = amount;
            else
                Tokens += amount;

            return this;
        }

        public ItemReward AddRandomTokens(int min, int max)
        {
            if (!Tokens.HasValue)
                Tokens = 0u;

            Tokens += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddChests(uint amount)
        {
            if (!Chests.HasValue)
                Chests = amount;
            else
                Chests += amount;

            return this;
        }

        public ItemReward AddRandomChests(int min, int max)
        {
            if (!Chests.HasValue)
                Chests = 0u;

            Chests += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddSpheres(uint amount)
        {
            if (!Spheres.HasValue)
                Spheres = amount;
            else
                Spheres += amount;

            return this;
        }

        public ItemReward AddRandomSpheres(int min, int max)
        {
            if (!Spheres.HasValue)
                Spheres = 0u;

            Spheres += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddCapsules(uint amount)
        {
            if (!Capsules.HasValue)
                Capsules = amount;
            else
                Capsules += amount;

            return this;
        }

        public ItemReward AddRandomCapsules(int min, int max)
        {
            if (!Capsules.HasValue)
                Capsules = 0u;

            Capsules += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddTickets(uint amount)
        {
            if (!Tickets.HasValue)
                Tickets = amount;
            else
                Tickets += amount;

            return this;
        }

        public ItemReward AddRandomTickets(int min, int max)
        {
            if (!Tickets.HasValue)
                Tickets = 0u;

            Tickets += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddDoubleExps(uint amount)
        {
            if (!DoubleExps.HasValue)
                DoubleExps = amount;
            else
                DoubleExps += amount;

            return this;
        }

        public ItemReward AddRandomDoubleExps(int min, int max)
        {
            if (!DoubleExps.HasValue)
                DoubleExps = 0u;

            DoubleExps += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddBotRespects(uint amount)
        {
            if (!BotRespects.HasValue)
                BotRespects = amount;
            else
                BotRespects += amount;

            return this;
        }

        public ItemReward AddRandomBotRespects(int min, int max)
        {
            if (!BotRespects.HasValue)
                BotRespects = 0u;

            BotRespects += Helper.NextUInt(min, max);
            return this;
        }

        public ItemReward AddRewinds(uint amount)
        {
            if (!Rewinds.HasValue)
                Rewinds = amount;
            else
                Rewinds += amount;

            return this;
        }

        public ItemReward AddRandomRewinds(int min, int max)
        {
            if (!Rewinds.HasValue)
                Rewinds = 0u;

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

            if (Coins.HasValue) sb.Append($"{emotes.GetEmoteString("$emotecoins")} {Coins.Value.ToString()} ");
            if (Tokens.HasValue) sb.Append($"{emotes.GetEmoteString("$emotetokens")} {Tokens.Value.ToString()} ");
            if (Chests.HasValue) sb.Append($"{emotes.GetEmoteString("$emotechest")} {Chests.Value.ToString()} ");
            if (Spheres.HasValue) sb.Append($"{emotes.GetEmoteString("$emotesphere")} {Spheres.Value.ToString()} ");
            if (Capsules.HasValue) sb.Append($"{emotes.GetEmoteString("$emotecapsule")} {Capsules.Value.ToString()} ");
            if (Tickets.HasValue) sb.Append($"{emotes.GetEmoteString("$emoteticket")} {Tickets.Value.ToString()} ");
            if (DoubleExps.HasValue) sb.Append($"{emotes.GetEmoteString("$emote2exp")} {DoubleExps.Value.ToString()} ");
            if (BotRespects.HasValue)
                sb.Append($"{emotes.GetEmoteString("$emoterespect")} {BotRespects.Value.ToString()} ");
            if (Rewinds.HasValue) sb.Append($"{emotes.GetEmoteString("$emoterewind")} {Rewinds.Value.ToString()} ");

            return sb.ToString().TrimEnd();
        }

        public string ToPlainString()
        {
            var sb = new StringBuilder();

            if (Coins.HasValue) sb.Append($"{nameof(Coins)} {Coins.Value.ToString()} ");
            if (Tokens.HasValue) sb.Append($"{nameof(Tokens)} {Tokens.Value.ToString()} ");
            if (Chests.HasValue) sb.Append($"{nameof(Chests)} {Chests.Value.ToString()} ");
            if (Spheres.HasValue) sb.Append($"{nameof(Spheres)} {Spheres.Value.ToString()} ");
            if (Capsules.HasValue) sb.Append($"{nameof(Capsules)} {Capsules.Value.ToString()} ");
            if (Tickets.HasValue) sb.Append($"{nameof(Tickets)} {Tickets.Value.ToString()} ");
            if (DoubleExps.HasValue) sb.Append($"{nameof(DoubleExps)} {DoubleExps.Value.ToString()} ");
            if (BotRespects.HasValue) sb.Append($"{nameof(BotRespects)} {BotRespects.Value.ToString()} ");
            if (Rewinds.HasValue) sb.Append($"{nameof(Rewinds)} {Rewinds.Value.ToString()} ");

            return sb.ToString().TrimEnd();
        }
    }
}
