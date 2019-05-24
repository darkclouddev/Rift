using System;

using Rift.Data.Models.Users;
using Rift.Services.Reward;

using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using MingweiSamuel.Camille.SummonerV4;

namespace Rift.Services.Message
{
    public class FormatData
    {
        public ulong UserId { get; set; }

        public EconomyData Economy { get; set; }

        public BotRespectData BotRespect { get; set; }

        public BragData Brag { get; set; }

        public LolStatData LolStat { get; set; }

        public ModerationData Moderation { get; set; }

        public FormatData() {}

        public FormatData(ulong userId)
        {
            UserId = userId;
        }
    }

    public class BotRespectData
    {
        public ItemReward Reward { get; set; }
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

    public class LolStatData
    {
        public Summoner Summoner { get; set; }
        public LeaguePosition SoloQueue { get; set; }
        public LeaguePosition Flex5v5 { get; set; }
    }

    public class ModerationData
    {
        public ulong ModeratorId { get; set; }
        public ulong TargetId { get; set; }
        public string Reason { get; set; }
        public TimeSpan? Duration { get; set; } = null;
    }
}
