using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Data.Models;
using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Riot;
using Rift.Util;

using Discord;
using Discord.WebSocket;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using IonicLib;
using IonicLib.Extensions;

using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using MingweiSamuel.Camille.SummonerV4;

using Newtonsoft.Json;

namespace Rift.Services
{
    public class RiotService
    {
        public static EventHandler<LolDataCreatedEventArgs> OnLolDataCreated;

        static Champion championData;
        static Dictionary<string, ChampionData> champions = new Dictionary<string, ChampionData>();

        static readonly string DataDragonDataFolder = Path.Combine(RiftBot.AppPath, "lol-data");
        static readonly string TempFolder = Path.Combine(DataDragonDataFolder, "temp");

        const string DataDragonVersionsUrl = "https://ddragon.leagueoflegends.com/api/versions.json";
        const string DataDragonTarballUrlTemplate = "https://ddragon.leagueoflegends.com/cdn/dragontail-{0}.tgz";
        const string DataDragonChampPortraitTemplate = "https://ddragon.leagueoflegends.com/cdn/{0}/img/champion/{1}";

        static Timer approveTimer;

        static readonly TimeSpan ApproveCheckCooldown = TimeSpan.FromMinutes(1);

        static readonly SemaphoreSlim RegisterMutex = new SemaphoreSlim(1);

        static RiotApi api;

        public RiotService()
        {
            if (!Directory.Exists(DataDragonDataFolder))
                Directory.CreateDirectory(DataDragonDataFolder);

            if (Directory.Exists(TempFolder))
                Directory.Delete(TempFolder, true);

            Directory.CreateDirectory(TempFolder);

            api = RiotApi.NewInstance(Settings.App.RiotApiKey);

            approveTimer = new Timer(
                async delegate { await CheckApproveAsync(); },
                null,
                TimeSpan.FromSeconds(20),
                ApproveCheckCooldown);
        }

        #region Data

        public async Task InitAsync()
        {
            await UpdateDataAsync();
        }

        static async Task UpdateDataAsync()
        {
            (var newDataAvailable, var version) = await CheckNewVersionAsync().ConfigureAwait(false);

            if (!newDataAvailable)
            {
                LoadData();
                return;
            }

            RiftBot.Log.Info($"Initiating data update: {Settings.App.LolVersion} -> {version}");

            await DownloadDataAsync(version).ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    RiftBot.Log.Error(x.Exception, "Data download failed!");
                    RiftBot.Log.Warn("Falling back to existing data.");
                    LoadData();

                    return;
                }

                var path = Path.Combine(TempFolder, $"{version}.tgz");

                await UnpackData(path, version).ContinueWith(async y =>
                {
                    if (!y.IsCompletedSuccessfully)
                    {
                        RiftBot.Log.Error(y.Exception, "Data download failed!");
                        LoadData();

                        return;
                    }

                    var tempPath = Path.Combine(TempFolder, version, "champion.json");
                    var realPath = Path.Combine(DataDragonDataFolder, "champion.json");

                    try
                    {
                        if (File.Exists(realPath))
                            File.Delete(realPath);

                        File.Move(tempPath, realPath);
                    }
                    catch (Exception ex)
                    {
                        RiftBot.Log.Error(ex);
                        return;
                    }

                    LoadData();

                    Settings.App.LolVersion = version;
                    await Settings.SaveAppAsync();

                    RiftBot.Log.Info($"Data update completed. New version is {Settings.App.LolVersion}");

                    await RiftBot.SendMessageToDevelopers(
                        $"Data update completed. New version is {Settings.App.LolVersion}.");
                });
            });
        }

        static async Task<(bool, string)> CheckNewVersionAsync()
        {
            var response = await GetAsync(DataDragonVersionsUrl);

            if (response.StatusCode != 200)
            {
                RiftBot.Log.Error($"Failed to execute GET request: {response.StatusCode.ToString()}");

                return (false, string.Empty);
            }

            var version = JsonConvert.DeserializeObject<List<string>>(response.Content).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(version))
            {
                RiftBot.Log.Warn($"Failed to parse data version!");

                return (false, string.Empty);
            }

            if (version.Equals(Settings.App.LolVersion, StringComparison.InvariantCultureIgnoreCase))
            {
                RiftBot.Log.Info($"Data version {Settings.App.LolVersion} is up to date.");

                return (false, string.Empty);
            }

            RiftBot.Log.Info($"Found new version: {version}");

            return (true, version);
        }

        static async Task DownloadDataAsync(string version)
        {
            using (var client = new WebClient())
            {
                var file = Path.Combine(TempFolder, $"{version}.tgz");

                if (File.Exists(file))
                    return;

                await client.DownloadFileTaskAsync(new Uri(string.Format(DataDragonTarballUrlTemplate, version)), file);
            }
        }

        static void LoadData()
        {
            var json = File.ReadAllText(Path.Combine(DataDragonDataFolder, "champion.json"));
            championData = JsonConvert.DeserializeObject<Champion>(json);

            champions.Clear();

            foreach (var champData in championData.Data) champions.Add(champData.Value.Key, champData.Value);

            RiftBot.Log.Info($"Loaded {champions.Count.ToString()} champion(s).");
        }

        static async Task UnpackData(string pathToArchive, string version)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(pathToArchive);
            var tarFile = Path.Combine(TempFolder, nameWithoutExtension + ".tar");
            var tempVersionFolder = Path.Combine(TempFolder, version);

            await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(tarFile))
                    {
                        var buffer = new byte[4096];

                        using (var gzip = new FileStream(pathToArchive, FileMode.Open, FileAccess.Read))
                        using (var gzipStream = new GZipInputStream(gzip))
                        using (var gzipOut = File.Create(tarFile))
                        {
                            StreamUtils.Copy(gzipStream, gzipOut, buffer);
                        }
                    }

                    using (var tar = new FileStream(tarFile, FileMode.Open, FileAccess.Read))
                    using (var tarIn = new TarInputStream(tar))
                    {
                        TarEntry entry;

                        while ((entry = tarIn.GetNextEntry()) != null)
                        {
                            if (entry.IsDirectory)
                                continue;

                            if (!entry.Name.Contains($"ru_RU/champion.json"))
                                continue;

                            var name = entry.Name.Replace('/', Path.DirectorySeparatorChar);

                            if (Path.IsPathRooted(name))
                                name = name.Substring(Path.GetPathRoot(name).Length);

                            Directory.CreateDirectory(tempVersionFolder);

                            using (var tarOut =
                                new FileStream(Path.Combine(tempVersionFolder, "champion.json"), FileMode.Create))
                            {
                                tarIn.CopyEntryContents(tarOut);
                            }

                            break;
                        }
                    }

                    File.Delete(pathToArchive);
                    File.Delete(tarFile);
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, "Failed to unpack downloaded data");

                    return;
                }
            });
        }

        public ChampionData GetChampionById(string id)
        {
            var champion =
                champions.Values.FirstOrDefault(x => x.Key.Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (champion is null)
                RiftBot.Log.Error($"Champion id \"{id}\" does not exist.");

            return champion;
        }

        #endregion Data

        #region Validation

        public async Task<IonicMessage> RegisterAsync(ulong userId, string region, string summonerName)
        {
            await RegisterMutex.WaitAsync().ConfigureAwait(false);

            try
            {
                return await RegisterInternalAsync(userId, region, summonerName);
            }
            finally
            {
                RegisterMutex.Release();
            }
        }

        async Task<IonicMessage> RegisterInternalAsync(ulong userId, string region, string summonerName)
        {
            if (await DB.LeagueData.HasAsync(userId))
                return await RiftBot.GetMessageAsync("register-hasAcc", new FormatData(userId));

            if (await DB.PendingUsers.IsPendingAsync(userId))
                return await RiftBot.GetMessageAsync("register-pending", new FormatData(userId));

            (var summonerResult, var summoner) = await GetSummonerByNameAsync(region.ToLowerInvariant(), summonerName);

            if (summonerResult != RequestResult.Success)
                return await RiftBot.GetMessageAsync("register-namenotfound", new FormatData(userId));

            if (await DB.LeagueData.IsTakenAsync(region, summoner.Id))
                return await RiftBot.GetMessageAsync("register-taken", new FormatData(userId));

            var code = Helper.GenerateConfirmationCode(16);

            var pendingUser = new RiftPendingUser
            {
                UserId = userId,
                Region = region.ToLowerInvariant(),
                PlayerUUID = summoner.Puuid,
                AccountId = summoner.AccountId,
                SummonedId = summoner.Id,
                ConfirmationCode = code,
                ExpirationTime = DateTime.UtcNow + Settings.Economy.PendingUserLifeTime
            };

            if (!await AddForApprovalAsync(pendingUser))
                return MessageService.Error;

            return await RiftBot.GetMessageAsync("register-code", new FormatData(userId)
            {
                LeagueRegistration = new LeagueRegistrationData
                {
                    ConfirmationCode = code
                }
            });
        }

        static async Task<bool> AddForApprovalAsync(RiftPendingUser pendingUser)
        {
            if (await DB.PendingUsers.IsPendingAsync(pendingUser.UserId))
                return false;

            await DB.PendingUsers.AddAsync(pendingUser);
            return true;
        }

        async Task CheckApproveAsync()
        {
            var pendingUsers = await DB.PendingUsers.GetAllAsync();

            if (pendingUsers is null || !pendingUsers.Any())
                return;

            RiftBot.Log.Debug($"Checking {pendingUsers.Count.ToString()} pending users...");

            foreach (var user in pendingUsers)
            {
                var expired = DateTime.UtcNow > user.ExpirationTime;

                if (expired)
                {
                    await DB.PendingUsers.RemoveAsync(user);
                    await RiftBot.SendMessageAsync("register-pending-expired", Settings.ChannelId.Commands, new FormatData(user.UserId));
                }
            }

            var usersValidated = 0;

            foreach (var user in pendingUsers)
            {
                RequestResult codeResult;
                string code;
                try
                {
                    (codeResult, code) =
                        await GetThirdPartyCodeByEncryptedSummonerIdAsync(user.Region, user.SummonedId);
                }
                catch (Exception)
                {
                    continue;
                }

                if (code is null || codeResult != RequestResult.Success)
                    continue;

                var sanitizedCode = new string(code.Where(char.IsLetterOrDigit).ToArray());

                if (sanitizedCode != user.ConfirmationCode)
                    continue;

                (var requestResult, var summoner) = await GetSummonerByEncryptedUUIDAsync(user.Region, user.PlayerUUID);

                if (requestResult != RequestResult.Success)
                {
                    await DB.PendingUsers.RemoveAsync(user);
                    continue;
                }

                OnLolDataCreated?.Invoke(null, new LolDataCreatedEventArgs(user.UserId));

                var lolData = new RiftLeagueData
                {
                    UserId = user.UserId,
                    SummonerRegion = user.Region,
                    PlayerUUID = user.PlayerUUID,
                    AccountId = user.AccountId,
                    SummonerId = user.SummonedId,
                    SummonerName = summoner.Name
                };

                await DB.LeagueData.AddAsync(lolData);

                usersValidated++;

                await PostValidateAsync(user);
            }

            RiftBot.Log.Info($"{usersValidated.ToString()} users validated.");
        }

        async Task PostValidateAsync(RiftPendingUser pendingUser)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, pendingUser.UserId);

            if (sgUser is null)
                return;

            await DB.PendingUsers.RemoveAsync(pendingUser.UserId);

            await AssignRoleFromRankAsync(sgUser, pendingUser);

            await DB.Inventory.AddAsync(sgUser.Id, new InventoryData {Chests = 2u});

            await RiftBot.SendMessageAsync("register-success", Settings.ChannelId.Commands, new FormatData(pendingUser.UserId));
        }

        #endregion Validation

        #region Rank

        public async Task UpdateSummonerAsync(ulong userId)
        {
            RiftBot.Log.Info($"[User|{userId.ToString()}] Getting summoner for the league data update");

            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
            {
                RiftBot.Log.Error($"{userId.ToString()} Failed to find guild user.");
                return;
            }
            
            var leagueData = await DB.LeagueData.GetAsync(userId);
            (var result, var data) = await GetSummonerByEncryptedSummonerIdAsync(leagueData.SummonerRegion, leagueData.SummonerId);
            
            if (result == RequestResult.Error)
            {
                RiftBot.Log.Error($"{userId.ToString()} League data update failed.");
                return;
            }

            await DB.LeagueData.UpdateAsync(userId, leagueData.SummonerRegion, leagueData.PlayerUUID, leagueData.AccountId,
                leagueData.SummonerId, data.Name);

            await UpdateRankRoleAsync(sgUser, leagueData);

            RiftBot.Log.Info($"{userId.ToString()} League data update completed.");
        }

        async Task UpdateRankRoleAsync(IGuildUser sgUser, RiftLeagueData leagueData)
        {
            (var rankResult, var rankData) =
                await GetLeaguePositionsByEncryptedSummonerIdAsync(leagueData.SummonerRegion, leagueData.SummonerId);
            
            if (rankResult != RequestResult.Success)
                return;
            
            var newRank = GetRankFromEntry(rankData.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5"));
            
            var currentRank = await GetCurrentRank(sgUser);

            if (currentRank == newRank)
            {
                RiftBot.Log.Info($"{sgUser.ToLogString()} Same rank, nothing to update.");
                return;
            }

            await RemoveRankedRole(sgUser, currentRank);
            
            var roleId = await GetRoleByLeagueRank(newRank);
            if (roleId is null || roleId.RoleId == 0ul)
                return;

            if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId.RoleId, out var role))
                return;

            await RiftBot.SendMessageAsync("rank-updated", Settings.ChannelId.Chat, new FormatData(sgUser.Id)
            {
                RankData = new RankData
                {
                    PreviousRank = GetStatStringFromRank(currentRank),
                    CurrentRank = GetStatStringFromRank(newRank)
                }
            });

            await sgUser.AddRoleAsync(role);
        }

        static async Task<RiftRole> GetRoleByLeagueRank(LeagueRank rank)
        {
            return rank switch
            {
                LeagueRank.Iron => await DB.Roles.GetAsync(58),
                LeagueRank.Bronze => await DB.Roles.GetAsync(25),
                LeagueRank.Silver => await DB.Roles.GetAsync(33),
                LeagueRank.Gold => await DB.Roles.GetAsync(3),
                LeagueRank.Platinum => await DB.Roles.GetAsync(11),
                LeagueRank.Diamond => await DB.Roles.GetAsync(8),
                LeagueRank.Master => await DB.Roles.GetAsync(79),
                LeagueRank.GrandMaster => await DB.Roles.GetAsync(71),
                LeagueRank.Challenger => await DB.Roles.GetAsync(23),
                _ => null
            };
        }

        static async Task<LeagueRank> GetCurrentRank(IGuildUser user)
        {
            var currentRoleIds = user.RoleIds;

            var iron = await DB.Roles.GetAsync(58);
            if (currentRoleIds.Contains(iron.RoleId))
                return LeagueRank.Iron;

            var bronze = await DB.Roles.GetAsync(25);
            if (currentRoleIds.Contains(bronze.RoleId))
                return LeagueRank.Bronze;

            var silver = await DB.Roles.GetAsync(33);
            if (currentRoleIds.Contains(silver.RoleId))
                return LeagueRank.Silver;

            var gold = await DB.Roles.GetAsync(3);
            if (currentRoleIds.Contains(gold.RoleId))
                return LeagueRank.Gold;

            var platinum = await DB.Roles.GetAsync(11);
            if (currentRoleIds.Contains(platinum.RoleId))
                return LeagueRank.Platinum;

            var diamond = await DB.Roles.GetAsync(8);
            if (currentRoleIds.Contains(diamond.RoleId))
                return LeagueRank.Diamond;

            var master = await DB.Roles.GetAsync(79);
            if (currentRoleIds.Contains(master.RoleId))
                return LeagueRank.Master;

            var grandmaster = await DB.Roles.GetAsync(71);
            if (currentRoleIds.Contains(grandmaster.RoleId))
                return LeagueRank.GrandMaster;

            var challenger = await DB.Roles.GetAsync(23);
            if (currentRoleIds.Contains(challenger.RoleId))
                return LeagueRank.Challenger;

            return LeagueRank.Unranked;
        }

        static async Task RemoveRankedRole(IGuildUser sgUser, LeagueRank rank)
        {
            switch (rank)
            {
                case LeagueRank.Iron:

                    var iron = await DB.Roles.GetAsync(58);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, iron.RoleId, out var ironRole))
                        return;

                    await sgUser.RemoveRoleAsync(ironRole);
                    break;

                case LeagueRank.Bronze:

                    var bronze = await DB.Roles.GetAsync(25);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, bronze.RoleId, out var bronzeRole))
                        return;

                    await sgUser.RemoveRoleAsync(bronzeRole);
                    break;

                case LeagueRank.Silver:

                    var silver = await DB.Roles.GetAsync(33);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, silver.RoleId, out var silverRole))
                        return;

                    await sgUser.RemoveRoleAsync(silverRole);
                    break;

                case LeagueRank.Gold:

                    var gold = await DB.Roles.GetAsync(3);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, gold.RoleId, out var goldRole))
                        return;

                    await sgUser.RemoveRoleAsync(goldRole);
                    break;

                case LeagueRank.Platinum:

                    var platinum = await DB.Roles.GetAsync(11);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, platinum.RoleId, out var platRole))
                        return;

                    await sgUser.RemoveRoleAsync(platRole);
                    break;

                case LeagueRank.Diamond:

                    var diamond = await DB.Roles.GetAsync(8);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, diamond.RoleId, out var diamondRole))
                        return;

                    await sgUser.RemoveRoleAsync(diamondRole);
                    break;

                case LeagueRank.Master:

                    var master = await DB.Roles.GetAsync(79);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, master.RoleId, out var masterRole))
                        return;

                    await sgUser.RemoveRoleAsync(masterRole);
                    break;

                case LeagueRank.GrandMaster:

                    var grandmaster = await DB.Roles.GetAsync(71);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, grandmaster.RoleId, out var grandmasterRole))
                        return;

                    await sgUser.RemoveRoleAsync(grandmasterRole);
                    break;

                case LeagueRank.Challenger:

                    var challenger = await DB.Roles.GetAsync(23);

                    if (!IonicClient.GetRole(Settings.App.MainGuildId, challenger.RoleId, out var challengerRole))
                        return;

                    await sgUser.RemoveRoleAsync(challengerRole);
                    break;
                
                default: break;
            }
        }

        async Task AssignRoleFromRankAsync(SocketGuildUser guildUser, RiftPendingUser approvedUser)
        {
            (var rankResult, var rankData) =
                await GetLeaguePositionsByEncryptedSummonerIdAsync(approvedUser.Region, approvedUser.SummonedId);

            if (rankResult != RequestResult.Success)
                return;

            var soloqRank = GetRankFromEntry(rankData.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5"));

            var roleId = await GetRoleByLeagueRank(soloqRank);
            if (roleId is null || roleId.RoleId == 0ul)
                return;

            if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId.RoleId, out var role))
                return;

            await guildUser.AddRoleAsync(role);
        }

        public static LeagueRank GetRankFromEntry(LeagueEntry entry)
        {
            if (entry is null)
                return LeagueRank.Unranked;

            return entry.Tier switch
            {
                "IRON" => LeagueRank.Iron,
                "BRONZE" => LeagueRank.Bronze,
                "SILVER" => LeagueRank.Silver,
                "GOLD" => LeagueRank.Gold,
                "PLATINUM" => LeagueRank.Platinum,
                "DIAMOND" => LeagueRank.Diamond,
                "MASTER" => LeagueRank.Master,
                "GRANDMASTER" => LeagueRank.GrandMaster,
                "CHALLENGER" => LeagueRank.Challenger,
                _ => LeagueRank.Unranked
            };
        }

        public static string GetStatStringFromRank(LeagueRank rank)
        {
            var emoteService = RiftBot.GetService<EmoteService>();

            switch (rank)
            {
                case LeagueRank.Iron: return $"{emoteService.GetEmoteString("$emoteRankIron")} Железо";
                case LeagueRank.Bronze: return $"{emoteService.GetEmoteString("$emoteRankBronze")} Бронза";
                case LeagueRank.Silver: return $"{emoteService.GetEmoteString("$emoteRankSilver")} Серебро";
                case LeagueRank.Gold: return $"{emoteService.GetEmoteString("$emoteRankGold")} Золото";
                case LeagueRank.Platinum: return $"{emoteService.GetEmoteString("$emoteRankPlatinum")} Платина";
                case LeagueRank.Diamond: return $"{emoteService.GetEmoteString("$emoteRankDiamond")} Алмаз";
                case LeagueRank.Master: return $"{emoteService.GetEmoteString("$emoteRankMaster")} Мастер";
                case LeagueRank.GrandMaster: return $"{emoteService.GetEmoteString("$emoteRankGrandmaster")} Грандмастер";
                case LeagueRank.Challenger: return $"{emoteService.GetEmoteString("$emoteRankChallenger")} Претендент";
            }

            return string.Empty;
        }

        #endregion Rank

        public async Task<IonicMessage> GetUserGameStatAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var dbSummoner = await DB.LeagueData.GetAsync(userId);

            if (string.IsNullOrWhiteSpace(dbSummoner.PlayerUUID))
                return await RiftBot.GetMessageAsync("loldata-nodata", new FormatData(userId));

            (var summonerResult, var summoner) = await RiftBot.GetService<RiotService>()
                .GetSummonerByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (summonerResult != RequestResult.Success)
                return MessageService.Error;

            (var requestResult, var leaguePositions) = await RiftBot.GetService<RiotService>()
                .GetLeaguePositionsByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (requestResult != RequestResult.Success)
                return MessageService.Error;

            return await RiftBot.GetMessageAsync("loldata-stat-success", new FormatData(userId)
            {
                LeagueStat = new LeagueStatData
                {
                    Summoner = summoner,
                    SoloQueue = leaguePositions.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5"),
                    Flex5v5 = leaguePositions.FirstOrDefault(x => x.QueueType == "RANKED_FLEX_SR"),
                }
            });
        }

        public async Task<(RequestResult, Summoner)> GetSummonerByEncryptedSummonerIdAsync(string region, string encryptedSummonerId)
        {
            try
            {
                var result =
                    await api.SummonerV4.GetBySummonerIdAsync(GetRegionFromString(region), encryptedSummonerId);

                return (RequestResult.Success, result);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, Summoner)> GetSummonerByEncryptedUUIDAsync(string region, string encryptedUUID)
        {
            try
            {
                var result = await api.SummonerV4.GetByPUUIDAsync(GetRegionFromString(region), encryptedUUID);

                return (RequestResult.Success, result);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, Summoner)> GetSummonerByNameAsync(string region, string name)
        {
            try
            {
                var result = await api.SummonerV4.GetBySummonerNameAsync(GetRegionFromString(region), name);

                return (RequestResult.Success, result);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, LeagueEntry[])> GetLeaguePositionsByEncryptedSummonerIdAsync(string region, string encryptedSummonerId)
        {
            try
            {
                var result = await api.LeagueV4.GetLeagueEntriesForSummonerAsync(GetRegionFromString(region), encryptedSummonerId);

                return (RequestResult.Success, result);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, string)> GetThirdPartyCodeByEncryptedSummonerIdAsync(string region, string encryptedSummonerId)
        {
            try
            {
                var result =
                    await api.ThirdPartyCodeV4.GetThirdPartyCodeBySummonerIdAsync(
                        GetRegionFromString(region), encryptedSummonerId);

                return (RequestResult.Success, result);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, MatchReference[])> GetLast20MatchesByAccountIdAsync(string region, string encryptedAccountId)
        {
            try
            {
                var matchlist =
                    await api.MatchV4.GetMatchlistAsync(GetRegionFromString(region), encryptedAccountId, beginIndex: 0, endIndex: 19);

                return (RequestResult.Success, matchlist.Matches);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        public async Task<(RequestResult, Match)> GetMatchById(string region, long matchId)
        {
            try
            {
                var match = await api.MatchV4.GetMatchAsync(GetRegionFromString(region), matchId);

                return (RequestResult.Success, match);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
                return (RequestResult.Error, null);
            }
        }

        static Region GetRegionFromString(string regionName)
        {
            switch (regionName.ToLowerInvariant())
            {
                case "ru": return Region.RU;
                case "euw": return Region.EUW;
                case "eune": return Region.EUNE;
                case "na": return Region.NA;

                default:
                    RiftBot.Log.Error($"Invalid region: {regionName}");
                    throw new NotSupportedException($"Invalid region: {regionName}");
            }
        }

        public string GetSummonerIconUrlById(int iconId)
        {
            return $"http://ddragon.leagueoflegends.com/cdn/{Settings.App.LolVersion}/img/profileicon/{iconId.ToString()}.png";
        }

        public static string GetChampionSquareByName(ChampionImage ci)
        {
            return string.Format(DataDragonChampPortraitTemplate, Settings.App.LolVersion, ci.Full);
        }

        public string GetQueueNameById(int id)
        {
            switch (id)
            {
                case 0: return "Кастомная игра";

                case 70:
                case 72:
                case 73:
                case 75:
                case 76:
                case 78:
                case 83:
                case 98:
                case 310:
                case 313:
                case 317:
                case 325:
                case 600:
                case 610:
                case 900:
                case 910:
                case 920:
                case 940:
                case 950:
                case 960:
                case 980:
                case 990:
                case 1000:
                case 1010:
                case 1020:
                case 1030:
                case 1040:
                case 1050:
                case 1060:
                case 1070: return "Специальный";

                case 400:
                case 430:
                case 460: return "Обычная очередь";

                case 420: return "Ранговая (одиночная)";

                case 440:
                case 470: return "Ранговая (гибкая)";

                case 100:
                case 450: return "ARAM";

                case 800:
                case 810:
                case 820:
                case 830:
                case 840:
                case 850: return "Катка с ботами";

                case 1200: return "Осада Нексуса";

                default:
                    RiftBot.Log.Warn($"Unknown queue ID: {id.ToString()}");
                    return "Неизвестная очередь";
            }
        }

        static async Task<GetResponse> GetAsync(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse) await request.GetResponseAsync();
            }
            catch (WebException we)
            {
                response = (HttpWebResponse) we.Response;
            }

            var statusCode = response.StatusCode;

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return new GetResponse((int) statusCode, await reader.ReadToEndAsync());
            }
        }
    }

    public enum RequestResult : byte
    {
        Success,
        Error,
    }

    public enum LeagueRank : byte
    {
        Unranked,
        Iron,
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Master,
        GrandMaster,
        Challenger,
    }

    public class GetResponse
    {
        public readonly int StatusCode;
        public readonly string Content;

        public GetResponse(int statusCode, string content)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }

    public class Champion
    {
        [JsonProperty("data")]
        public Dictionary<string, ChampionData> Data { get; set; }
    }

    public class ChampionData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("image")]
        public ChampionImage Image { get; set; }
    }

    public class ChampionImage
    {
        [JsonProperty("full")]
        public string Full { get; set; }
    }
}
