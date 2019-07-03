using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataRankSolo : TemplateBase
    {
        public LeagueDataRankSolo() : base(nameof(LeagueDataRankSolo)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            if (data.LolStat.SoloQueue is null)
                return ReplaceData(message, "Недостаточно игр");

            var league = data.LolStat.SoloQueue;
            var totalGames = league.Wins + league.Losses;
            var winRatePerc = (int)Math.Round(((double)league.Wins / (double)totalGames) * 100);
            var leagueName = $"{RiotService.GetStatStringFromRank(RiotService.GetRankFromPosition(league))} {league.Rank}";

            return ReplaceData(message, $"{leagueName} ({league.LeaguePoints.ToString()}LP / " +
                                        $"{league.Wins.ToString()}W {league.Losses.ToString()}L) ({winRatePerc.ToString()}%)");
        }
    }
}
