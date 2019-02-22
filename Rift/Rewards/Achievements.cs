namespace Rift.Rewards
{
    public static class Achievements
    {
        public static readonly Reward Write100Messages = new Reward(chests: 2, powerupsDoubleExp: 1);

        public static readonly Reward Write1000Messages = new Reward(chests: 6, powerupsBotRespect: 1);

        public static readonly Reward Reach10Level = new Reward(chests: 10);

        public static readonly Reward Reach30Level = new Reward(coins: 10_000);

        public static readonly Reward Brag100Times = new Reward(chests: 10, powerupsDoubleExp: 1, powerupsBotRespect: 1);

        public static readonly Reward Attack200Times = new Reward(chests: 10);

        public static readonly Reward OpenSphere = new Reward(chests: 4);

        public static readonly Reward Purchase200Items = new Reward(chests: 4, giveawayTickets: 1);

        public static readonly Reward Open100Chests = new Reward(chests: 4, tokens: 1);

        public static readonly Reward Send100Gifts = new Reward(powerupsDoubleExp: 2, powerupsBotRespect: 2);

        public static readonly Reward IsDonator = new Reward(spheres: 1);

        public static readonly Reward HasDonatedRole = new Reward(chests: 10);

        public static readonly Reward GiftToBotKeeper = new Reward(customTickets: 2, tokens: 1);

        public static readonly Reward GiftToModerator = new Reward(tokens: 1, giveawayTickets: 2);

        public static readonly Reward AttackWise = new Reward(chests: 4, tokens: 1);
    }
}
