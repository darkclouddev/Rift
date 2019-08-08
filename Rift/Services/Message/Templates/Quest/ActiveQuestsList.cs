using System;
using System.Text;
using System.Threading.Tasks;

using Humanizer;

using Rift.Attributes;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class ActiveQuestsList : TemplateBase
    {
        public ActiveQuestsList() : base(nameof(ActiveQuestsList))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var emoteService = RiftBot.GetService<EmoteService>();
            var finishedEmote = emoteService.GetEmoteString("$emoteach");
            var notFinishedEmote = emoteService.GetEmoteString("$emotenoach");
            var quest = data.Quest;
            var progress = data.QuestProgress;
            var sb = new StringBuilder();
            var questType = quest.GetType();
            
            if (quest.ApprovedLolAccount.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.ApprovedLolAccount));
                
                if (progress.ApprovedLolAccount.HasValue)
                {
                    sb.AppendLine(progress.ApprovedLolAccount.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                {
                    sb.AppendLine(notFinishedEmote + description);
                }
            }
            
            if (quest.BragsDone.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.BragsDone));
                description = string.Format(description, quest.BragsDone.Value);
                var pr = GetProgress(progress.BragsDone, quest.BragsDone.Value);
                
                if (progress.BragsDone.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.BragsDone.Value >= quest.BragsDone
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.MessagesSent.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.MessagesSent));
                description = string.Format(description, quest.MessagesSent.Value);
                var pr = GetProgress(progress.MessagesSent, quest.MessagesSent.Value);
                
                if (progress.MessagesSent.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.MessagesSent.Value >= quest.MessagesSent
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.BoughtChests.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.BoughtChests));
                description = string.Format(description, quest.BoughtChests.Value);
                var pr = GetProgress(progress.BoughtChests, quest.BoughtChests.Value);
                
                if (progress.BoughtChests.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.BoughtChests.Value >= quest.BoughtChests
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.OpenedChests.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.OpenedChests));
                description = string.Format(description, quest.OpenedChests.Value);
                var pr = GetProgress(progress.OpenedChests, quest.OpenedChests.Value);
                
                if (progress.OpenedChests.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.OpenedChests.Value >= quest.OpenedChests
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.NormalMonstersKilled.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.NormalMonstersKilled));
                description = string.Format(description, quest.NormalMonstersKilled.Value);
                var pr = GetProgress(progress.NormalMonstersKilled, quest.NormalMonstersKilled.Value);
                
                if (progress.NormalMonstersKilled.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.NormalMonstersKilled.Value >= quest.NormalMonstersKilled
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.RareMonstersKilled.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.RareMonstersKilled));
                description = string.Format(description, quest.RareMonstersKilled.Value);
                var pr = GetProgress(progress.RareMonstersKilled, quest.RareMonstersKilled.Value);
                
                if (progress.RareMonstersKilled.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.RareMonstersKilled.Value >= quest.RareMonstersKilled
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.EpicMonstersKilled.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.EpicMonstersKilled));
                description = string.Format(description, quest.EpicMonstersKilled.Value);
                var pr = GetProgress(progress.EpicMonstersKilled, quest.EpicMonstersKilled.Value);
                
                if (progress.EpicMonstersKilled.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.EpicMonstersKilled.Value >= quest.EpicMonstersKilled
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.GiftsSent.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftsSent));
                description = string.Format(description, quest.GiftsSent.Value);
                var pr = GetProgress(progress.GiftsSent, quest.GiftsSent.Value);
                
                if (progress.GiftsSent.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.GiftsSent.Value >= quest.GiftsSent
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.GiftsReceived.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftsReceived));
                description = string.Format(description, quest.GiftsReceived.Value);
                var pr = GetProgress(progress.GiftsReceived, quest.GiftsReceived.Value);
                
                if (progress.GiftsReceived.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.GiftsReceived.Value >= quest.GiftsReceived
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.GiftedFounder.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftedFounder));
                description = string.Format(description, quest.GiftedFounder.Value);
                
                if (progress.GiftedFounder.HasValue)
                {
                    sb.AppendLine(progress.GiftedFounder.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description}");
            }
            
            if (quest.LevelReached.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.LevelReached));
                description = string.Format(description, quest.LevelReached.Value);
                var pr = GetProgress(progress.LevelReached, quest.LevelReached.Value);
                
                if (progress.LevelReached.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.LevelReached.Value >= quest.LevelReached
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.GiveawaysParticipated.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiveawaysParticipated));
                description = string.Format(description, quest.GiveawaysParticipated.Value);
                var pr = GetProgress(progress.GiveawaysParticipated, quest.GiveawaysParticipated.Value);
                
                if (progress.GiveawaysParticipated.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.GiveawaysParticipated.Value >= quest.GiveawaysParticipated
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.CoinsReceived.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.CoinsReceived));
                description = string.Format(description, quest.CoinsReceived.Value);
                var pr = GetProgress(progress.CoinsReceived, quest.CoinsReceived.Value);
                
                if (progress.CoinsReceived.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.CoinsReceived.Value >= quest.CoinsReceived
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.CoinsSpent.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.CoinsSpent));
                description = string.Format(description, quest.CoinsSpent.Value);
                var pr = GetProgress(progress.CoinsSpent, quest.CoinsSpent.Value);
                
                if (progress.CoinsSpent.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.CoinsSpent.Value >= quest.CoinsSpent
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.VoiceUptimeEarned.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.VoiceUptimeEarned));
                description = string.Format(description, quest.VoiceUptimeEarned.Value.Humanize(culture: RiftBot.Culture));
                var pr = GetProgress(progress.VoiceUptimeEarned);
                
                if (progress.VoiceUptimeEarned.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.VoiceUptimeEarned.Value >= quest.VoiceUptimeEarned
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.GiftedDeveloper.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftedDeveloper));
                description = string.Format(description, quest.GiftedDeveloper.Value);
                
                if (progress.GiftedDeveloper.HasValue)
                {
                    sb.AppendLine(progress.GiftedDeveloper.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description}");
            }
            
            if (quest.GiftedModerator.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftedModerator));
                description = string.Format(description, quest.GiftedModerator.Value);
                
                if (progress.GiftedModerator.HasValue)
                {
                    sb.AppendLine(progress.GiftedModerator.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description}");
            }
            
            if (quest.GiftedStreamer.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.GiftedStreamer));
                description = string.Format(description, quest.GiftedStreamer.Value);
                
                if (progress.GiftedStreamer.HasValue)
                {
                    sb.AppendLine(progress.GiftedStreamer.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description}");
            }
            
            if (quest.ActivatedBotRespects.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.ActivatedBotRespects));
                description = string.Format(description, quest.ActivatedBotRespects.Value);
                var pr = GetProgress(progress.ActivatedBotRespects, quest.ActivatedBotRespects.Value);
                
                if (progress.ActivatedBotRespects.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.ActivatedBotRespects.Value >= quest.ActivatedBotRespects
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }
            
            if (quest.OpenedSphere.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.OpenedSphere));
                description = string.Format(description, quest.OpenedSphere.Value);
                
                if (progress.OpenedSphere.HasValue)
                {
                    sb.AppendLine(progress.OpenedSphere.Value
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description}");
            }
            
            if (quest.RolesPurchased.HasValue)
            {
                var description = GetDescription(questType, nameof(quest.RolesPurchased));
                description = string.Format(description, quest.RolesPurchased.Value);
                var pr = GetProgress(progress.RolesPurchased, quest.RolesPurchased.Value);
                
                if (progress.RolesPurchased.HasValue)
                {
                    description = $"{description} {pr}";
                    sb.AppendLine(progress.RolesPurchased.Value >= quest.RolesPurchased
                        ? finishedEmote + description
                        : notFinishedEmote + description
                    );
                }
                else
                    sb.AppendLine($"{notFinishedEmote} {description} {pr}");
            }

            if (sb.Length == 0)
            {
                TemplateError("Active quest contains zero conditions!");
                return Task.FromResult(message);
            }

            return ReplaceDataAsync(message, sb.ToString());
        }

        static string GetDescription(Type type, string propertyName)
        {
            var props = type.GetProperty(propertyName).GetCustomAttributes(true);

            foreach (var prop in props)
            {
                if (prop is QuestConditionAttribute attr)
                    return attr.Description;
            }

            return string.Empty;
        }

        static string GetProgress(uint? current, uint goal)
        {
            if (!current.HasValue)
                return $"(0/{goal.ToString()})";

            if (current >= goal)
                current = goal;
            
            return $"({current.Value.ToString()}/{goal.ToString()})";
        }

        static string GetProgress(TimeSpan? current)
        {
            return !current.HasValue
                ? "(0 часов)"
                : $"({current.Value.Humanize(culture: RiftBot.Culture)})";
        }
    }
}
