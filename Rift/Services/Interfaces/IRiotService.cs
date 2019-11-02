using System;
using System.Threading.Tasks;

using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using MingweiSamuel.Camille.SummonerV4;

using Rift.Events;
using Rift.Services.Message;

namespace Rift.Services.Interfaces
{
    public interface IRiotService
    {
        event EventHandler<LolDataCreatedEventArgs> OnLolDataCreated;
        Task InitAsync();
        ChampionData GetChampionById(string id);
        Task<IonicMessage> RegisterAsync(ulong userId, string region, string summonerName);
        Task UpdateSummonerAsync(ulong userId);
        LeagueRank GetRankFromEntry(LeagueEntry entry);
        string GetStatStringFromRank(LeagueRank rank);
        Task<IonicMessage> GetUserGameStatAsync(ulong userId);
        Task<(RequestResult, Summoner)> GetSummonerByEncryptedSummonerIdAsync(string region, string encryptedSummonerId);
        Task<(RequestResult, Summoner)> GetSummonerByEncryptedUuidAsync(string region, string encryptedUuid);
        Task<(RequestResult, Summoner)> GetSummonerByNameAsync(string region, string name);
        Task<(RequestResult, LeagueEntry[])> GetLeaguePositionsByEncryptedSummonerIdAsync(string region, string encryptedSummonerId);
        Task<(RequestResult, string)> GetThirdPartyCodeByEncryptedSummonerIdAsync(string region, string encryptedSummonerId);
        Task<(RequestResult, MatchReference[])> GetLast20MatchesByAccountIdAsync(string region, string encryptedAccountId);
        Task<(RequestResult, Match)> GetMatchById(string region, long matchId);
        string GetSummonerIconUrlById(int iconId);
        string GetChampionSquareByName(ChampionImage ci);
        string GetQueueNameById(int id);
    }
}
