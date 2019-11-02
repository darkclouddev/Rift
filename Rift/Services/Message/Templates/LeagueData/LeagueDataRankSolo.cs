using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataRankSolo : TemplateBase
    {
        public LeagueDataRankSolo() : base(nameof(LeagueDataRankSolo))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (data.LeagueStat.SoloQueue is null)
                return ReplaceDataAsync(message, "Недостаточно игр");

            var league = data.LeagueStat.SoloQueue;
            var totalGames = league.Wins + league.Losses;
            var winRatePerc = (int) Math.Round((double) league.Wins / (double) totalGames * 100);
            var leagueName = $"{data.RiotService.GetStatStringFromRank(data.RiotService.GetRankFromEntry(league))} {league.Rank}";

            return ReplaceDataAsync(message, $"{leagueName} ({league.LeaguePoints.ToString()}LP / " +
                                             $"{league.Wins.ToString()}W {league.Losses.ToString()}L) ({winRatePerc.ToString()}%)");
        }
    }
}
