using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataRankFlex : TemplateBase
    {
        public LeagueDataRankFlex() : base(nameof(LeagueDataRankFlex))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (data.LeagueStat.Flex5v5 is null)
                return ReplaceDataAsync(message, "Недостаточно игр");

            var league = data.LeagueStat.Flex5v5;
            var totalGames = league.Wins + league.Losses;
            var winRatePerc = (int) Math.Round((double) league.Wins / (double) totalGames * 100);
            var leagueName =
                $"{RiotService.GetStatStringFromRank(RiotService.GetRankFromPosition(league))} {league.Rank}";

            return ReplaceDataAsync(message, $"{leagueName} ({league.LeaguePoints.ToString()}LP / " +
                                             $"{league.Wins.ToString()}W {league.Losses.ToString()}L) ({winRatePerc.ToString()}%)");
        }
    }
}
