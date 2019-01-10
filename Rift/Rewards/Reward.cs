using System;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services;

namespace Rift.Rewards
{
    public class Reward
    {
        protected Loot CoinsLoot { get; set; }
        protected Loot TokensLoot { get; set; }
        protected Loot ChestsLoot { get; set; }
        protected Loot SpheresLoot { get; set; }
        protected Loot CapsulesLoot { get; set; }
        protected Loot PowerupsDoubleExpLoot { get; set; }
        protected Loot PowerupsBotRespectLoot { get; set; }
        protected Loot CustomTicketsLoot { get; set; }
        protected Loot GiveawayTicketsLoot { get; set; }
        protected Loot ExperienceLoot { get; set; }

        public uint Coins { get; protected set; } = 0u;
        public uint Tokens { get; protected set; } = 0u;
        public uint Chests { get; protected set; } = 0u;
        public uint Spheres { get; protected set; } = 0u;
        public uint Capsules { get; protected set; } = 0u;
        public uint PowerupsDoubleExp { get; protected set; } = 0u;
        public uint PowerupsBotRespect { get; protected set; } = 0u;
        public uint UsualTickets { get; protected set; } = 0u;
        public uint RareTickets { get; protected set; } = 0u;
        public uint Experience { get; protected set; } = 0u;

        public string RewardString { get; set; } = "";

        public virtual void GenerateRewardString()
        {
            if (String.IsNullOrWhiteSpace(RewardString))
            {
                if (Experience > 0u)
                    RewardString += $" {Settings.Emote.Experience} {Experience}";

                if (Coins > 0u)
                    RewardString += $" {Settings.Emote.Coin} {Coins}";

                if (Tokens > 0u)
                    RewardString += $" {Settings.Emote.Token} {Tokens}";

                if (Chests > 0u)
                    RewardString += $" {Settings.Emote.Chest} {Chests}";

                if (Spheres > 0u)
                    RewardString += $" {Settings.Emote.Sphere} {Spheres}";

                if (Capsules > 0u)
                    RewardString += $" {Settings.Emote.Capsule} {Capsules}";

                if (PowerupsDoubleExp > 0u)
                    RewardString += $" {Settings.Emote.PowerupDoubleExperience} {PowerupsDoubleExp}";

                if (PowerupsBotRespect > 0u)
                    RewardString += $" {Settings.Emote.BotRespect} {PowerupsBotRespect}";

                if (UsualTickets > 0u)
                    RewardString += $" {Settings.Emote.Ctickets} {UsualTickets}";

                if (RareTickets > 0u)
                    RewardString += $" {Settings.Emote.Gtickets} {RareTickets}";

                RewardString = RewardString.TrimStart();
            }
        }

        public virtual async Task GiveRewardAsync(ulong userId)
        {
            await RiftBot.GetService<DatabaseService>()
                         .AddInventoryAsync(userId,
                                            coins: this.Coins,
                                            tokens: this.Tokens,
                                            chests: this.Chests,
                                            spheres: this.Spheres,
                                            capsules: this.Capsules,
                                            doubleExps: this.PowerupsDoubleExp,
                                            respects: this.PowerupsBotRespect,
                                            usualTickets: this.UsualTickets,
                                            rareTickets: this.RareTickets);

            await RiftBot.GetService<DatabaseService>().AddExperienceAsync(userId, Experience);
        }

        public Reward(Loot coins = null, Loot tokens = null, Loot chests = null, Loot spheres = null,
                      Loot capsules = null,
                      Loot powerupsDoubleExp = null, Loot powerupsBotRespect = null,
                      Loot customTickets = null, Loot giveawayTickets = null,
                      Loot experience = null)
        {
            CoinsLoot = coins;
            TokensLoot = tokens;
            ChestsLoot = chests;
            SpheresLoot = spheres;
            CapsulesLoot = capsules;
            PowerupsDoubleExpLoot = powerupsDoubleExp;
            PowerupsBotRespectLoot = powerupsBotRespect;
            CustomTicketsLoot = customTickets;
            GiveawayTicketsLoot = giveawayTickets;
            ExperienceLoot = experience;
        }

        public Reward(uint coins = 0u, uint tokens = 0u, uint chests = 0u, uint spheres = 0u, uint capsules = 0u,
                      uint powerupsProtection = 0u, uint powerupsDoubleExp = 0u, uint powerupsBotRespect = 0u,
                      uint customTickets = 0u, uint giveawayTickets = 0u, uint experience = 0u)
        {
            CoinsLoot = coins > 0u ? new Loot(coins) : null;
            TokensLoot = tokens > 0u ? new Loot(tokens) : null;
            ChestsLoot = chests > 0u ? new Loot(chests) : null;
            SpheresLoot = spheres > 0u ? new Loot(spheres) : null;
            CapsulesLoot = capsules > 0u ? new Loot(capsules) : null;
            PowerupsDoubleExpLoot = powerupsDoubleExp > 0u ? new Loot(powerupsDoubleExp) : null;
            PowerupsBotRespectLoot = powerupsBotRespect > 0u ? new Loot(powerupsBotRespect) : null;
            CustomTicketsLoot = customTickets > 0u ? new Loot(customTickets) : null;
            GiveawayTicketsLoot = giveawayTickets > 0u ? new Loot(giveawayTickets) : null;
            ExperienceLoot = experience > 0u ? new Loot(experience) : null;
        }

        public void CalculateReward()
        {
            if (CoinsLoot != null)
                Coins = CoinsLoot.CalculateLoot().Count;

            if (TokensLoot != null)
                Tokens = TokensLoot.CalculateLoot().Count;

            if (ChestsLoot != null)
                Chests = ChestsLoot.CalculateLoot().Count;

            if (SpheresLoot != null)
                Spheres = SpheresLoot.CalculateLoot().Count;

            if (CapsulesLoot != null)
                Capsules = CapsulesLoot.CalculateLoot().Count;

            if (PowerupsDoubleExpLoot != null)
                PowerupsDoubleExp = PowerupsDoubleExpLoot.CalculateLoot().Count;

            if (PowerupsBotRespectLoot != null)
                PowerupsBotRespect = PowerupsBotRespectLoot.CalculateLoot().Count;

            if (CustomTicketsLoot != null)
                UsualTickets = CustomTicketsLoot.CalculateLoot().Count;

            if (GiveawayTicketsLoot != null)
                RareTickets = GiveawayTicketsLoot.CalculateLoot().Count;

            if (ExperienceLoot != null)
                Experience = ExperienceLoot.CalculateLoot().Count;
        }

        public static Reward operator +(Reward r1, Reward r2)
        {
            r1.CalculateReward();
            r2.CalculateReward();
            return new Reward(coins: r1.Coins + r2.Coins
                              , tokens: r1.Tokens + r2.Tokens
                              , chests: r1.Chests + r2.Chests
                              , spheres: r1.Spheres + r2.Spheres
                              , capsules: r1.Capsules + r2.Capsules
                              , powerupsDoubleExp: r1.PowerupsDoubleExp + r2.PowerupsDoubleExp
                              , powerupsBotRespect: r1.PowerupsBotRespect + r2.PowerupsBotRespect
                              , customTickets: r1.UsualTickets + r2.UsualTickets
                              , giveawayTickets: r1.RareTickets + r2.RareTickets
                              , experience: r1.Experience + r2.Experience);
        }

        public Reward Copy()
        {
            CalculateReward();

            return new Reward(coins: (Coins > 0) ? new Loot(Coins) : null
                              , tokens: (Tokens > 0) ? new Loot(Tokens) : null
                              , chests: (Chests > 0) ? new Loot(Chests) : null
                              , spheres: (Spheres > 0) ? new Loot(Spheres) : null
                              , capsules: (Capsules > 0) ? new Loot(Capsules) : null
                              , powerupsDoubleExp: (PowerupsDoubleExp > 0) ? new Loot(PowerupsDoubleExp) : null
                              , powerupsBotRespect: (PowerupsBotRespect > 0) ? new Loot(PowerupsBotRespect) : null
                              , customTickets: (UsualTickets > 0) ? new Loot(UsualTickets) : null
                              , giveawayTickets: (RareTickets > 0) ? new Loot(RareTickets) : null
                              , experience: (Experience > 0) ? new Loot(Experience) : null);
        }
    }
}
