using System;
using System.Collections.Generic;

using Rift.Data.Models;
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
        public BragData Brag { get; set; }
        public EconomyData Economy { get; set; }
        public EventData EventData { get; set; }
        public GiftData Gift { get; set; }
        public GiveawayData Giveaway { get; set; }
        public LeagueRegistrationData LeagueRegistration { get; set; }
        public LeagueStatData LeagueStat { get; set; }
        public ModerationData Moderation { get; set; }
        public RewardBase Reward { get; set; }
        public RiftQuest Quest { get; set; }
        public RiftStage QuestStage { get; set; }
        public RiftQuestProgress QuestProgress { get; set; }
        public RiftStatistics Statistics { get; set; }
        public VoteData VoteData { get; set; }

        public FormatData(ulong userId)
        {
            UserId = userId;
        }
    }

    public class BragData
    {
        public ParticipantStats Stats { get; set; }
        public string QueueName { get; set; }
        public string ChampionName { get; set; }
        public string ChampionPortraitUrl { get; set; }
    }

    public class EconomyData
    {
        public List<UserTopExp> Top10Exp { get; set; }
        public List<UserTopCoins> Top10Coins { get; set; }
    }

    public class EventData
    {
        public RiftActiveEvent Active { get; set; }
        public RiftEvent Stored { get; set; }
        public RiftEventLog Log { get; set; }
    }

    public class GiftData
    {
        public ulong TargetId { get; set; }
        public TimeSpan Cooldown { get; set; }
        public uint NecessaryCoins { get; set; }
    }

    public class GiveawayData
    {
        public RiftActiveGiveaway Active { get; set; }
        public RiftGiveaway Stored { get; set; }
        public RiftGiveawayLog Log { get; set; }
        public TicketGiveaway TicketGiveaway { get; set; }
    }

    public class LeagueRegistrationData
    {
        public string ConfirmationCode { get; set; }
    }

    public class LeagueStatData
    {
        public Summoner Summoner { get; set; }
        public LeagueEntry SoloQueue { get; set; }
        public LeagueEntry Flex5v5 { get; set; }
    }

    public class ModerationData
    {
        public ulong ModeratorId { get; set; }
        public ulong TargetId { get; set; }
        public string Reason { get; set; }
        public TimeSpan? Duration { get; set; } = null;

        public static implicit operator ModerationData(RiftModerationLog log)
        {
            return new ModerationData
            {
                ModeratorId = log.ModeratorId,
                TargetId = log.TargetId,
                Reason = log.Reason,
                Duration = log.Duration != TimeSpan.Zero ? log.Duration : TimeSpan.Zero
            };
        }
    }

    public class TicketGiveaway
    {
        public int ParticipantsCount { get; set; }
        public ulong WinnerId { get; set; }
    }

    public class VoteData
    {
        public string Name { get; set; }
    }
}
