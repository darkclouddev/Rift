namespace Rift.Configuration
{
    public class Economy
    {
        public uint MessageCooldownSeconds { get; set; } = 0u;
        public uint GiftCooldownSeconds { get; set; } = 0u;
        public uint StoreCooldownSeconds { get; set; } = 0u;
        public uint AttackPerUserCooldownSeconds { get; set; } = 0u;
        public uint AttackSameUserCooldownSeconds { get; set; } = 0u;
        public uint AttackPrice { get; set; } = 0u;
        public uint AttackMinimumLevel { get; set; } = 0u;
        public int AttackMinimumCoins { get; set; } = 0;
        public int AttackMaximumCoins { get; set; } = 0;
        public int AttackMuteDurationMinimumSeconds { get; set; } = 0;
        public int AttackMuteDurationMaximumSeconds { get; set; } = 0;
        public string LastDonateId { get; set; } = "";
        public ulong BragCooldownSeconds { get; set; } = 0u;
        public int MagicSetCoinsMinimum { get; set; } = 0;
        public int MagicSetCoinsMaximum { get; set; } = 0;
        public int MagicSetTokensMinimum { get; set; } = 0;
        public int MagicSetTokensMaximum { get; set; } = 0;
        public int BragWinCoinsMin { get; set; } = 0;
        public int BragWinCoinsMax { get; set; } = 0;
        public int BragLossCoinsMin { get; set; } = 0;
        public int BragLossCoinsMax { get; set; } = 0;
    }
}
