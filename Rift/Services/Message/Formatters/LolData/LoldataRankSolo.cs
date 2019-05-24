﻿using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LoldataRankSolo : FormatterBase
    {
        public LoldataRankSolo() : base("$loldataRankSolo") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
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
