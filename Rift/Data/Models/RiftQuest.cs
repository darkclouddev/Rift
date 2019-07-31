using System;

using Rift.Attributes;

namespace Rift.Data.Models
{
    public class RiftQuest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StageId { get; set; }
        public int RewardId { get; set; }
        public int Order { get; set; }
        
        [QuestCondition("Подтвердить игровой аккаунт")]
        public bool? ApprovedLolAccount { get; set; }
        
        [QuestCondition("Похвастаться {0} раз")]
        public uint? BragsDone { get; set; }
        
        [QuestCondition("Написать {0} сообщений")]
        public uint? MessagesSent { get; set; }
        
        [QuestCondition("Купить {0} сундуков")]
        public uint? BoughtChests { get; set; }
        
        [QuestCondition("Открыть {0} сундуков")]
        public uint? OpenedChests { get; set; }
        
        [QuestCondition("Убить {0} обычных монстров")]
        public uint? NormalMonstersKilled { get; set; }
        
        [QuestCondition("Убить {0} редких монстров")]
        public uint? RareMonstersKilled { get; set; }
        
        [QuestCondition("Убить {0} эпических монстров")]
        public uint? EpicMonstersKilled { get; set; }
        
        [QuestCondition("Отправить {0} подарков")]
        public uint? GiftsSent { get; set; }
        
        [QuestCondition("Получить {0} подарков")]
        public uint? GiftsReceived { get; set; }
        
        [QuestCondition("Получить подарок от основателя")]
        public uint? GiftsReceivedFromUltraGay { get; set; }
        
        [QuestCondition("Достичь {0} уровня")]
        public uint? LevelReached { get; set; }
        
        [QuestCondition("Принять участие в {0} розыгрышах")]
        public uint? GiveawaysParticipated { get; set; }
        
        [QuestCondition("Заработать {0} монет")]
        public uint? CoinsReceived { get; set; }
        
        [QuestCondition("Потратить {0} монет")]
        public uint? CoinsSpent { get; set; }
        
        [QuestCondition("Просидеть {0} в голосовых чатах")]
        public TimeSpan? VoiceUptimeEarned { get; set; }
        
        [QuestCondition("Подарить подарок разработчику")]
        public bool? GiftedDeveloper { get; set; }
        
        [QuestCondition("Подарить подарок модератору")]
        public bool? GiftedModerator { get; set; }
        
        [QuestCondition("Подарить подарок любимому стримеру")]
        public bool? GiftedStreamer { get; set; }
        
        [QuestCondition("Активировать {0} уважений ботов")]
        public uint? ActivatedBotRespects { get; set; }
        
        [QuestCondition("Открыть сферу")]
        public bool? OpenedSphere { get; set; }
        
        [QuestCondition("Купить {0} ролей")]
        public uint? RolesPurchased { get; set; }

        public bool IsCompleted(RiftQuestProgress progress)
        {
            if (ApprovedLolAccount.HasValue)
            {
                if (!progress.ApprovedLolAccount.HasValue)
                    return false;
                
                if (ApprovedLolAccount.Value && !progress.ApprovedLolAccount.Value)
                    return false;
            }

            if (BragsDone.HasValue)
            {
                if (!progress.BragsDone.HasValue)
                    return false;

                if (BragsDone.Value > progress.BragsDone.Value)
                    return false;
            }

            if (MessagesSent.HasValue)
            {
                if (!progress.MessagesSent.HasValue)
                    return false;

                if (MessagesSent.Value > progress.MessagesSent.Value)
                    return false;
            }

            if (BoughtChests.HasValue)
            {
                if (!progress.BoughtChests.HasValue)
                    return false;

                if (BoughtChests.Value > progress.BoughtChests.Value)
                    return false;
            }

            if (OpenedChests.HasValue)
            {
                if (!progress.OpenedChests.HasValue)
                    return false;

                if (OpenedChests.Value > progress.OpenedChests.Value)
                    return false;
            }

            if (NormalMonstersKilled.HasValue)
            {
                if (!progress.NormalMonstersKilled.HasValue)
                    return false;

                if (NormalMonstersKilled.Value > progress.NormalMonstersKilled.Value)
                    return false;
            }

            if (RareMonstersKilled.HasValue)
            {
                if (!progress.RareMonstersKilled.HasValue)
                    return false;

                if (RareMonstersKilled.Value > progress.RareMonstersKilled.Value)
                    return false;
            }

            if (EpicMonstersKilled.HasValue)
            {
                if (!progress.EpicMonstersKilled.HasValue)
                    return false;

                if (EpicMonstersKilled.Value > progress.EpicMonstersKilled.Value)
                    return false;
            }

            if (GiftsSent.HasValue)
            {
                if (!progress.GiftsSent.HasValue)
                    return false;

                if (GiftsSent.Value > progress.GiftsSent.Value)
                    return false;
            }

            if (GiftsReceived.HasValue)
            {
                if (!progress.GiftsReceived.HasValue)
                    return false;

                if (GiftsReceived.Value > progress.GiftsReceived.Value)
                    return false;
            }

            if (GiftsReceivedFromUltraGay.HasValue)
            {
                if (!progress.GiftsReceivedFromUltraGay.HasValue)
                    return false;

                if (GiftsReceivedFromUltraGay.Value > progress.GiftsReceivedFromUltraGay.Value)
                    return false;
            }

            if (LevelReached.HasValue)
            {
                if (!progress.LevelReached.HasValue)
                    return false;

                if (LevelReached.Value > progress.LevelReached.Value)
                    return false;
            }

            if (GiveawaysParticipated.HasValue)
            {
                if (!progress.GiveawaysParticipated.HasValue)
                    return false;

                if (GiveawaysParticipated.Value > progress.GiveawaysParticipated.Value)
                    return false;
            }

            if (CoinsReceived.HasValue)
            {
                if (!progress.CoinsReceived.HasValue)
                    return false;

                if (CoinsReceived.Value > progress.CoinsReceived.Value)
                    return false;
            }

            if (CoinsSpent.HasValue)
            {
                if (!progress.CoinsSpent.HasValue)
                    return false;

                if (CoinsSpent.Value > progress.CoinsSpent.Value)
                    return false;
            }

            if (VoiceUptimeEarned.HasValue)
            {
                if (!progress.VoiceUptimeEarned.HasValue)
                    return false;

                if (VoiceUptimeEarned.Value > progress.VoiceUptimeEarned.Value)
                    return false;
            }

            if (GiftedDeveloper.HasValue)
            {
                if (!progress.GiftedDeveloper.HasValue)
                    return false;

                if (GiftedDeveloper.Value && !progress.GiftedDeveloper.Value)
                    return false;
            }

            if (GiftedModerator.HasValue)
            {
                if (!progress.GiftedModerator.HasValue)
                    return false;

                if (GiftedModerator.Value && !progress.GiftedModerator.Value)
                    return false;
            }

            if (GiftedStreamer.HasValue)
            {
                if (!progress.GiftedStreamer.HasValue)
                    return false;

                if (GiftedStreamer.Value && !progress.GiftedStreamer.Value)
                    return false;
            }

            if (ActivatedBotRespects.HasValue)
            {
                if (!progress.ActivatedBotRespects.HasValue)
                    return false;

                if (ActivatedBotRespects.Value > progress.ActivatedBotRespects.Value)
                    return false;
            }

            if (OpenedSphere.HasValue)
            {
                if (!progress.OpenedSphere.HasValue)
                    return false;

                if (OpenedSphere.Value && !progress.OpenedSphere.Value)
                    return false;
            }

            if (RolesPurchased.HasValue)
            {
                if (!progress.RolesPurchased.HasValue)
                    return false;

                if (RolesPurchased.Value > progress.RolesPurchased.Value)
                    return false;
            }

            return true;
        }
    }
}
