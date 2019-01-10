namespace Rift.Rewards
{
    public class Achievements
    {
        public static Reward Write100Messages = new Reward(chests: 2, powerupsDoubleExp: 1);

        public static Reward Write1000Messages = new Reward(chests: 6, powerupsBotRespect: 1);

        public static Reward Reach10Level = new Reward(chests: 10);

        public static Reward Reach30Level = new Reward(coins: 10_000);

        public static Reward Brag100Times = new Reward(chests: 10, powerupsDoubleExp: 1, powerupsBotRespect: 1);

        public static Reward Attack200Times = new Reward(chests: 10);

        public static Reward OpenSphere = new Reward(chests: 4);

        public static Reward Purchase200Items = new Reward(chests: 4, giveawayTickets: 1);

        public static Reward Open100Chests = new Reward(chests: 4, tokens: 1);

        public static Reward Send100Gifts = new Reward(powerupsDoubleExp: 2, powerupsBotRespect: 2);

        public static Reward IsDonator = new Reward(spheres: 1);

        public static Reward HasDonatedRole = new Reward(chests: 10);

        public static Reward GiftToBotKeeper = new Reward(customTickets: 2, tokens: 1);

        public static Reward GiftToModerator = new Reward(tokens: 1, giveawayTickets: 2);

        public static Reward AttackWise = new Reward(chests: 4, tokens: 1);
    }
}
