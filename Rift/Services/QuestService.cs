using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Events;
using Rift.Services.Message;

namespace Rift.Services
{
    public class QuestService
    {
        const string QuestCompletionMessageName = "quest-completed";
        const string StageCompletionMessageName = "stage-completed";
        const string StageCompletionNoRewardMessageName = "stage-completed-noreward";
        
        static readonly SemaphoreSlim QuestFinishMutex = new SemaphoreSlim(1);

        public QuestService()
        {
            RiotService.OnLolDataCreated += OnLolDataCreated;
            DB.Users.OnLevelReached += OnLevelReached;
            BragService.OnUserBrag += OnUserBrag;
            DB.Statistics.OnMessageCreated += OnMessageCreated;
            ChestService.ChestsOpened += OnChestsOpened;
            GiftService.GiftSent += OnGiftSent;
            GiftService.GiftReceived += OnGiftReceived;
            GiftService.GiftedFounder += OnGiftedFounder;
            GiftService.GiftedDeveloper += OnGiftedDeveloper;
            GiftService.GiftedModerator += OnGiftedModerator;
            GiftService.GiftedStreamer += OnGiftedStreamer;
            DB.Inventory.OnCoinsReceived += OnCoinsReceived;
            DB.Inventory.OnCoinsSpent += OnCoinsSpent;
            DB.Statistics.VoiceUptimeEarned += OnVoiceUptimeEarned;
            BotRespectService.ActivatedBotRespects += OnActivatedBotRespects;
            SphereService.OpenedSphere += OnOpenedSphere;
            StoreService.BoughtChests += OnBoughtChests;
            StoreService.RolesPurchased += OnRolesPurchased;
            GiveawayService.GiveawaysParticipated += OnGiveawaysParticipated;
            EventService.NormalMonstersKilled += OnNormalMonstersKilled;
            EventService.RareMonstersKilled += OnRareMonstersKilled;
            EventService.EpicMonstersKilled += OnEpicMonstersKilled;
        }

        public async Task GetUserQuests(ulong userId)
        {
            var stageId = (await DB.Quests.GetActiveStageIdsAsync()).FirstOrDefault();
            if (stageId == 0)
            {
                await RiftBot.SendMessageAsync("user-noquests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var stage = await DB.Quests.GetStageAsync(stageId);
            if (stage is null)
            {
                RiftBot.Log.Error($"No stage data found! (ID {stageId.ToString()})");
                await RiftBot.SendMessageAsync("user-noquests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var questProgress = await DB.Quests.GetLastQuestProgressAsync(userId, stageId);
            if (questProgress is null)
            {
                await TryAddFirstQuestAsync(userId).ContinueWith(async _ =>
                {
                    questProgress = await DB.Quests.GetLastQuestProgressAsync(userId, stageId);
                });
            }

            var quest = await DB.Quests.GetQuestAsync(questProgress.QuestId);
            if (quest is null)
            {
                RiftBot.Log.Error($"Unable to get quest data! (ID {questProgress.QuestId.ToString()})");
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }

            await RiftBot.SendMessageAsync("user-quests", Settings.ChannelId.Commands, new FormatData(userId)
            {
                QuestStage = stage,
                QuestProgress = questProgress,
                Quest = quest
            });
        }

        public async Task TryAddFirstQuestAsync(ulong userId)
        {
            var activeStages = await DB.Quests.GetActiveStageIdsAsync();

            if (activeStages is null || activeStages.Count == 0)
            {
                //RiftBot.Log.Debug("No active stages atm.");
                return;
            }

            var stagesInProgress = await DB.Quests.GetStageIdsInProgressAsync(userId);

            var stagesToAdd = activeStages.Except(stagesInProgress).ToList();

            if (stagesToAdd.Count == 0)
            {
                //RiftBot.Log.Debug("No missing stages atm.");
                return;
            }

            foreach (var stageId in stagesToAdd)
            {
                if (await DB.Quests.HasCompletedStageAsync(userId, stageId))
                    continue;
                
                var firstQuest = await DB.Quests.GetFirstQuestAsync(stageId);

                if (firstQuest is null)
                {
                    RiftBot.Log.Debug($"No first quest with order 0 in stage ID {stageId.ToString()}.");
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(new RiftQuestProgress
                {
                    UserId = userId,
                    QuestId = firstQuest.Id,
                    IsCompleted = false
                });
            }
        }

        static async Task FinishQuestAsync(RiftQuest quest, RiftQuestProgress progress)
        {
            await QuestFinishMutex.WaitAsync();

            try
            {
                progress.IsCompleted = true;
                await DB.Quests.SetQuestsProgressAsync(progress);
            }
            finally
            {
                QuestFinishMutex.Release();
            }

            var data = new FormatData(progress.UserId)
            {
                Quest = quest
            };

            var dbReward = await DB.Rewards.GetAsync(quest.RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Reward {quest.RewardId.ToString()} does not exist in database!");
                return;
            }

            var reward = dbReward.ToRewardBase();
            await reward.DeliverToAsync(progress.UserId);
            data.Reward = reward;

            await RiftBot.SendMessageAsync(QuestCompletionMessageName, Settings.ChannelId.Commands, data);

            if (!await TryAddNextQuest(progress.UserId, quest))
            {
                // our quest was the last in stage
                await FinishStageAsync(progress.UserId, quest.StageId);
                return;
            }
        }

        static async Task<bool> TryAddNextQuest(ulong userId, RiftQuest completed)
        {
            var nextQuest = await DB.Quests.GetNextQuestInStage(completed.StageId, completed.Order);

            if (nextQuest is null)
                return false;

            await DB.Quests.SetQuestsProgressAsync(new RiftQuestProgress
            {
                UserId = userId,
                QuestId = nextQuest.Id,
                IsCompleted = false
            });

            return true;
        }

        static async Task FinishStageAsync(ulong userId, int stageId)
        {
            var stage = await DB.Quests.GetStageAsync(stageId);

            if (stage is null)
            {
                RiftBot.Log.Error($"Stage ID {stageId.ToString()} does not exist in database!");
                return;
            }

            IonicMessage msg;

            if (!stage.CompletionRewardId.HasValue)
            {
                msg = await RiftBot.GetMessageAsync(StageCompletionNoRewardMessageName, new FormatData(userId)
                {
                    QuestStage = stage
                });
            }
            else
            {
                var reward = await DB.Rewards.GetAsync(stage.CompletionRewardId.Value);

                if (reward is null)
                {
                    RiftBot.Log.Error($"Reward {stage.CompletionRewardId.Value.ToString()} does not exist in database!");
                    return;
                }

                var data = new FormatData(userId)
                {
                    Reward = reward.ToRewardBase(),
                    QuestStage = stage
                };

                await data.Reward.DeliverToAsync(userId);

                msg = await RiftBot.GetMessageAsync(StageCompletionMessageName, data);
            }

            await RiftBot.SendMessageAsync(msg, Settings.ChannelId.Commands);
        }

        static async void OnLevelReached(object sender, LevelReachedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.LevelReached is null)
                    continue;

                questProgress.LevelReached = e.Level;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnLolDataCreated(object sender, LolDataCreatedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.ApprovedLolAccount is null)
                    continue;

                questProgress.ApprovedLolAccount = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnUserBrag(object sender, BragEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.BragsDone is null)
                    continue;

                questProgress.BragsDone = (questProgress.BragsDone ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnMessageCreated(object sender, MessageCreatedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.MessagesSent is null)
                    continue;

                questProgress.MessagesSent = (questProgress.MessagesSent ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnChestsOpened(object sender, ChestsOpenedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.OpenedChests is null)
                    continue;

                questProgress.OpenedChests = (questProgress.OpenedChests ?? 0u) + e.Amount;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftSent(object sender, GiftSentEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftsSent is null)
                    continue;

                questProgress.GiftsSent = (questProgress.GiftsSent ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftReceived(object sender, GiftReceivedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftsReceived is null)
                    continue;

                questProgress.GiftsReceived = (questProgress.GiftsReceived ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftedFounder(object sender, GiftedFounderEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftedFounder is null)
                    continue;

                questProgress.GiftedFounder = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftedDeveloper(object sender, GiftedDeveloperEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftedDeveloper is null)
                    continue;

                questProgress.GiftedDeveloper = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftedModerator(object sender, GiftedModeratorEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftedModerator is null)
                    continue;

                questProgress.GiftedModerator = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiftedStreamer(object sender, GiftedStreamerEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftedStreamer is null)
                    continue;

                questProgress.GiftedStreamer = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnCoinsReceived(object sender, CoinsReceivedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.CoinsReceived is null)
                    continue;

                questProgress.CoinsReceived = (questProgress.CoinsReceived ?? 0u) + e.Amount;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnCoinsSpent(object sender, CoinsSpentEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.CoinsSpent is null)
                    continue;

                questProgress.CoinsSpent = (questProgress.CoinsSpent ?? 0u) + e.Amount;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnVoiceUptimeEarned(object sender, VoiceUptimeEarnedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.VoiceUptimeEarned is null)
                    continue;

                questProgress.VoiceUptimeEarned = (questProgress.VoiceUptimeEarned ?? TimeSpan.Zero) + e.Uptime;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnActivatedBotRespects(object sender, ActivatedBotRespectsEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.ActivatedBotRespects is null)
                    continue;

                questProgress.ActivatedBotRespects = (questProgress.ActivatedBotRespects ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnOpenedSphere(object sender, OpenedSphereEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.OpenedSphere is null)
                    continue;

                questProgress.OpenedSphere = true;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnBoughtChests(object sender, BoughtChestsEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.BoughtChests is null)
                    continue;

                questProgress.BoughtChests = (questProgress.BoughtChests ?? 0u) + e.Amount;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnRolesPurchased(object sender, RolesPurchasedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.RolesPurchased is null)
                    continue;

                questProgress.RolesPurchased = (questProgress.RolesPurchased ?? 0u) + e.Amount;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnGiveawaysParticipated(object sender, GiveawaysParticipatedEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiveawaysParticipated is null)
                    continue;

                questProgress.GiveawaysParticipated = (questProgress.GiveawaysParticipated ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnNormalMonstersKilled(object sender, NormalMonstersKilledEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.NormalMonstersKilled is null)
                    continue;

                questProgress.NormalMonstersKilled = (questProgress.NormalMonstersKilled ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnRareMonstersKilled(object sender, RareMonstersKilledEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.RareMonstersKilled is null)
                    continue;

                questProgress.RareMonstersKilled = (questProgress.RareMonstersKilled ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }

        static async void OnEpicMonstersKilled(object sender, EpicMonstersKilledEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.EpicMonstersKilled is null)
                    continue;

                questProgress.EpicMonstersKilled = (questProgress.EpicMonstersKilled ?? 0u) + 1u;

                if (dbQuest.IsCompleted(questProgress))
                {
                    await FinishQuestAsync(dbQuest, questProgress);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(questProgress);
            }
        }
    }
}
