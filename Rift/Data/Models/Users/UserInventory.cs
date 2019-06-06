using System.Text;

namespace Rift.Data.Models.Users
{
    public class UserInventory
    {
        public ulong UserId { get; set; }

        public uint Coins { get; set; } = 0u;

        public uint Tokens { get; set; } = 0u;

        public uint Essence { get; set; } = 0u;

        public uint Chests { get; set; } = 0u;

        public uint Spheres { get; set; } = 0u;

        public uint Capsules { get; set; } = 0u;

        public uint Tickets { get; set; } = 0u;

        public uint BonusDoubleExp { get; set; } = 0u;

        public uint BonusBotRespect { get; set; } = 0u;

        public uint BonusRewind { get; set; } = 0u;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Coins > 0u) sb.Append($"$emotecoins {Coins.ToString()} ");
            if (Tokens > 0u) sb.Append($"$emotetokens {Tokens.ToString()} ");
            if (Chests > 0u) sb.Append($"$emotechest {Chests.ToString()} ");
            if (Spheres > 0u) sb.Append($"$emotesphere {Spheres.ToString()} ");
            if (Capsules > 0u) sb.Append($"$emotecapsule {Capsules.ToString()} ");
            if (Tickets > 0u) sb.Append($"$emoteticket {Tickets.ToString()} ");
            if (BonusDoubleExp > 0u) sb.Append($"$emote2exp {BonusDoubleExp.ToString()} ");
            if (BonusBotRespect > 0u) sb.Append($"$emoterespect {BonusBotRespect.ToString()} ");
            if (BonusRewind > 0u) sb.Append($"$emoterewind {BonusRewind.ToString()} ");

            return sb.ToString().TrimEnd();
        }
    }
}
