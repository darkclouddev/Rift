using System;

using Rift.Rewards;

using MingweiSamuel.Camille.MatchV4;
using Rift.Data.Models.Users;

namespace Rift.Services.Message
{
    public class FormatData
    {
        public ulong UserId { get; set; }

        public EconomyData Economy { get; set; }

        public BotRespectData BotRespect { get; set; }

        public BragData Brag { get; set; }

        public ModerationData Moderation { get; set; }

        public FormatData() {}

        public FormatData(ulong userId)
        {
            UserId = userId;
        }
    }

    public class BotRespectData
    {
        public Reward Reward { get; set; }
    }

    public class BragData
    {
        public ParticipantStats Stats { get; set; }
        public string QueueName { get; set; }
        public string ChampionName { get; set; }
        public string ChampionPortraitUrl { get; set; }
        public string Reward { get; set; }
    }

    public class EconomyData
    {
        public UserTopExp[] Top10Exp { get; set; }
        public UserTopCoins[] Top10Coins { get; set; }
    }

    public class ModerationData
    {
        public ulong ModeratorId { get; set; }
        public ulong TargetId { get; set; }
        public string Reason { get; set; }
        public TimeSpan? Duration { get; set; } = null;
    }
}
