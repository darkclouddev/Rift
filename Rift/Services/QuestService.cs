using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Events;
using Rift.Services.Message;

namespace Rift.Services
{
    public class QuestService
    {
        const string CompletionMessageName = "quest-completed";

        public QuestService()
        {
            RiotService.OnLolDataCreated += OnLolDataCreated;
            DB.Users.OnLevelReached += OnLevelReached;
            BragService.OnUserBrag += OnUserBrag;
            DB.Statistics.OnMessageCreated += OnMessageCreated;
            EconomyService.ChestsOpened += OnChestsOpened;
            GiftService.GiftSent += OnGiftSent;
            GiftService.GiftReceived += OnGiftReceived;
            GiftService.GiftsReceivedFromFounder += OnGiftReceivedFromFounder;
            GiftService.GiftedBotKeeper += OnGiftedBotKeeper;
            GiftService.GiftedModerator += OnGiftedModerator;
            GiftService.GiftedStreamer += OnGiftedStreamer;
            DB.Inventory.OnCoinsReceived += OnCoinsReceived;
            DB.Inventory.OnCoinsSpent += OnCoinsSpent;
            DB.Statistics.VoiceUptimeEarned += OnVoiceUptimeEarned;
            EconomyService.ActivatedBotRespects += OnActivatedBotRespects;
            EconomyService.OpenedSphere += OnOpenedSphere;
            StoreService.BoughtChests += OnBoughtChests;
            StoreService.RolesPurchased += OnRolesPurchased;
            GiveawayService.GiveawaysParticipated += OnGiveawaysParticipated;
            //EventService.UsualMonstersKilled += OnUsualMonstersKilled;
            //EventService.RareMonstersKilled += OnRareMonstersKilled;
            //EventService.EpicMonstersKilled += OnEpicMonstersKilled;
        }

        public async Task AddNextQuestAsync(ulong userId)
        {
            var quests = await DB.Quests.GetAllStagedQuestsAsync();

            if (quests is null || quests.Count == 0)
            {
                RiftBot.Log.Debug("No quests in system.");
                return;
            }

            var userQuests = await DB.Quests.GetActiveQuestsProgressAsync(userId)
                             ?? new List<RiftQuestProgress>();

            foreach (var stage in quests)
            {
                if (!stage.Stage.IsInProgress())
                    continue;

                if (stage.Quests is null || stage.Quests.Count == 0)
                    continue;

                foreach (var quest in stage.Quests)
                    if (userQuests.All(x => x.QuestId != quest.Id))
                    {
                        await DB.Quests.AddQuestProgressAsync(userId, new RiftQuestProgress
                        {
                            UserId = userId,
                            QuestId = quest.Id,
                            IsCompleted = false
                        });
                        break;
                    }
            }
        }

        static async Task FinishQuestAsync(RiftQuest quest, RiftQuestProgress progress)
        {
            progress.IsCompleted = true;
            await DB.Quests.SetQuestsProgressAsync(progress);

            var data = new FormatData(progress.UserId)
            {
                Quest = quest
            };

            if (!quest.RewardId.HasValue)
            {
                RiftBot.Log.Error($"Quest {quest.Id.ToString()} has no reward to give out!");
                return;
            }

            var reward = await DB.Rewards.GetAsync(quest.RewardId.Value);

            if (reward is null)
            {
                RiftBot.Log.Error($"Reward {quest.RewardId.Value.ToString()} does not exist in database!");
                return;
            }

            if (!(reward.ItemReward is null))
            {
                data.Reward = reward.ItemReward;
                await RiftBot.SendChatMessageAsync(CompletionMessageName, data);
                await reward.ItemReward.DeliverToAsync(progress.UserId);
            }

            if (!(reward.RoleReward is null))
            {
                data.Reward = reward.RoleReward;
                await RiftBot.SendChatMessageAsync(CompletionMessageName, data);
                await reward.RoleReward.DeliverToAsync(progress.UserId);
            }
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    LevelReached = e.Level
                };

                if (rqp.LevelReached >= dbQuest.LevelReached.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    ApprovedLolAccount = true
                };

                if (rqp.ApprovedLolAccount.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    BragsDone = (questProgress.BragsDone ?? 0u) + 1u
                };

                if (rqp.BragsDone >= dbQuest.BragsDone.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    MessagesSent = (questProgress.MessagesSent ?? 0u) + 1u
                };

                if (rqp.MessagesSent >= dbQuest.MessagesSent.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    OpenedChests = (questProgress.OpenedChests ?? 0u) + e.Amount
                };

                if (rqp.OpenedChests >= dbQuest.OpenedChests.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftsSent = (questProgress.GiftsSent ?? 0u) + 1u
                };

                if (rqp.GiftsSent >= dbQuest.GiftsSent.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftsReceived = (questProgress.GiftsReceived ?? 0u) + 1u
                };

                if (rqp.GiftsReceived >= dbQuest.GiftsReceived.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
            }
        }

        static async void OnGiftReceivedFromFounder(object sender, GiftsReceivedFromFounderEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftsReceivedFromUltraGay is null)
                    continue;

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftsReceivedFromUltraGay = (questProgress.GiftsReceivedFromUltraGay ?? 0u) + 1u
                };

                if (rqp.GiftsReceivedFromUltraGay >= dbQuest.GiftsReceivedFromUltraGay)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
            }
        }

        static async void OnGiftedBotKeeper(object sender, GiftedBotKeeperEventArgs e)
        {
            var quests = await DB.Quests.GetActiveQuestsProgressAsync(e.UserId);

            if (quests is null || quests.Count == 0)
                return;

            foreach (var questProgress in quests)
            {
                var dbQuest = await DB.Quests.GetQuestAsync(questProgress.QuestId);

                if (dbQuest?.GiftedBotKeeper is null)
                    continue;

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftedBotKeeper = true
                };

                if (rqp.GiftedBotKeeper.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftedModerator = true
                };

                if (rqp.GiftedModerator.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiftedStreamer = true
                };

                if (rqp.GiftedStreamer.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    CoinsReceived = (questProgress.CoinsReceived ?? 0u) + e.Amount
                };

                if (rqp.CoinsReceived >= dbQuest.CoinsReceived.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    CoinsSpent = (questProgress.CoinsSpent ?? 0u) + e.Amount
                };

                if (rqp.CoinsSpent >= dbQuest.CoinsSpent.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    VoiceUptimeEarned = (questProgress.VoiceUptimeEarned ?? TimeSpan.Zero) + e.Uptime
                };

                if (rqp.VoiceUptimeEarned >= dbQuest.VoiceUptimeEarned.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    ActivatedBotRespects = (questProgress.ActivatedBotRespects ?? 0u) + 1u
                };

                if (rqp.ActivatedBotRespects >= dbQuest.ActivatedBotRespects.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    OpenedSphere = true
                };

                if (rqp.OpenedSphere.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    BoughtChests = (questProgress.BoughtChests ?? 0u) + e.Amount
                };

                if (rqp.BoughtChests.Value >= dbQuest.BoughtChests.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    RolesPurchased = (questProgress.RolesPurchased ?? 0u) + e.Amount
                };

                if (rqp.RolesPurchased.Value >= dbQuest.RolesPurchased.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
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

                var rqp = new RiftQuestProgress
                {
                    UserId = e.UserId,
                    QuestId = dbQuest.Id,
                    GiveawaysParticipated = (questProgress.GiveawaysParticipated ?? 0u) + 1u
                };

                if (rqp.GiveawaysParticipated.Value >= dbQuest.GiveawaysParticipated.Value)
                {
                    await FinishQuestAsync(dbQuest, rqp);
                    continue;
                }

                await DB.Quests.SetQuestsProgressAsync(rqp);
            }
        }
    }
}
