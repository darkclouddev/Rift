using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Quests
    {
        public async Task AddQuestProgressAsync(ulong userId, RiftQuestProgress userQuest)
        {
            if (!await DB.Users.EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(AddQuestProgressAsync));
                
            var dbUserQuest = await GetQuestProgressAsync(userId, userQuest.QuestId);
            var dbQuest = await GetQuestAsync(userQuest.QuestId);

            if (dbQuest.LevelReached.HasValue)
                userQuest.LevelReached = await DB.Users.GetLevelAsync(userId);

            if (dbQuest.ApprovedLolAccount.HasValue)
                userQuest.ApprovedLolAccount = await DB.LeagueData.HasAsync(userId);

            if (!(dbUserQuest is null))
                return;

            await using var context = new RiftContext();
            await context.QuestProgress.AddAsync(userQuest);
            await context.SaveChangesAsync();
        }

        public async Task<RiftQuest> GetFirstQuestAsync(int stageId)
        {
            await using var context = new RiftContext();
            return await context.Quests
                .Where(x => x.StageId == stageId && x.Order == 0)
                .FirstOrDefaultAsync();
        }

        public async Task<RiftQuest> GetLastQuestAsync(int stageId)
        {
            await using var context = new RiftContext();
            return await context.Quests
                .Where(x => x.StageId == stageId)
                .OrderByDescending(x => x.Order)
                .FirstOrDefaultAsync();
        }

        public async Task<RiftQuestProgress> GetLastQuestProgressAsync(ulong userId, int stageId)
        {
            await using var context = new RiftContext();
            var query =
                from stage in context.QuestStages
                join quest in context.Quests on stage.Id equals quest.StageId
                join progress in context.QuestProgress on quest.Id equals progress.QuestId
                where progress.UserId == userId && stage.IsInProgress()
                orderby quest.Order descending
                select progress;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<RiftQuestStage> GetStageAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.QuestStages
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> GetStageQuestCountAsync(int stageId)
        {
            await using var context = new RiftContext();
            return await context.Quests
                .Where(x => x.StageId == stageId)
                .CountAsync();
        }

        public async Task<List<int>> GetActiveStageIdsAsync()
        {
            await using var context = new RiftContext();
            var dt = DateTime.UtcNow;

            return await context.QuestStages
                .Where(x => x.StartDate < dt && dt <= x.EndDate)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToListAsync();
        }

        public async Task<RiftQuestProgress> GetQuestProgressAsync(ulong userId, int questId)
        {
            await using var context = new RiftContext();
            return await context.QuestProgress
                .FirstOrDefaultAsync(x => x.UserId == userId && x.QuestId == questId);
        }

        public async Task<List<RiftQuestProgress>> GetActiveQuestsProgressAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.QuestProgress
                .Where(x => x.UserId == userId && !x.IsCompleted)
                .ToListAsync();
        }

        public async Task<List<int>> GetStageIdsInProgressAsync(ulong userId)
        {
            await using var context = new RiftContext();
            var query =
                from stage in context.QuestStages
                join quest in context.Quests on stage.Id equals quest.StageId
                join progress in context.QuestProgress on quest.Id equals progress.QuestId
                where progress.UserId == userId && !progress.IsCompleted && stage.IsInProgress()
                orderby stage.Id
                select stage.Id;

            return await query.Distinct().ToListAsync();
        }

        public async Task<RiftQuest> GetQuestAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.Quests
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SetQuestsProgressAsync(RiftQuestProgress userQuest)
        {
            var dbUserQuest = await GetQuestProgressAsync(userQuest.UserId, userQuest.QuestId);

            if (dbUserQuest is null)
            {
                await AddQuestProgressAsync(userQuest.UserId, userQuest);
                return;
            }

            await using var context = new RiftContext();
            var entry = context.Entry(userQuest);

            if (dbUserQuest.IsCompleted != userQuest.IsCompleted)
                entry.Property(x => x.IsCompleted).IsModified = true;

            if (dbUserQuest.ApprovedLolAccount != userQuest.ApprovedLolAccount)
                entry.Property(x => x.ApprovedLolAccount).IsModified = true;

            if (dbUserQuest.BragsDone != userQuest.BragsDone)
                entry.Property(x => x.BragsDone).IsModified = true;

            if (dbUserQuest.MessagesSent != userQuest.MessagesSent)
                entry.Property(x => x.MessagesSent).IsModified = true;

            if (dbUserQuest.BoughtChests != userQuest.BoughtChests)
                entry.Property(x => x.BoughtChests).IsModified = true;

            if (dbUserQuest.OpenedChests != userQuest.OpenedChests)
                entry.Property(x => x.OpenedChests).IsModified = true;

            if (dbUserQuest.NormalMonstersKilled != userQuest.NormalMonstersKilled)
                entry.Property(x => x.NormalMonstersKilled).IsModified = true;

            if (dbUserQuest.RareMonstersKilled != userQuest.RareMonstersKilled)
                entry.Property(x => x.RareMonstersKilled).IsModified = true;

            if (dbUserQuest.EpicMonstersKilled != userQuest.EpicMonstersKilled)
                entry.Property(x => x.EpicMonstersKilled).IsModified = true;

            if (dbUserQuest.GiftsSent != userQuest.GiftsSent)
                entry.Property(x => x.GiftsSent).IsModified = true;

            if (dbUserQuest.GiftsReceived != userQuest.GiftsReceived)
                entry.Property(x => x.GiftsReceived).IsModified = true;

            if (dbUserQuest.GiftedFounder != userQuest.GiftedFounder)
                entry.Property(x => x.GiftedFounder).IsModified = true;

            if (dbUserQuest.LevelReached != userQuest.LevelReached)
                entry.Property(x => x.LevelReached).IsModified = true;

            if (dbUserQuest.GiveawaysParticipated != userQuest.GiveawaysParticipated)
                entry.Property(x => x.GiveawaysParticipated).IsModified = true;

            if (dbUserQuest.CoinsReceived != userQuest.CoinsReceived)
                entry.Property(x => x.CoinsReceived).IsModified = true;

            if (dbUserQuest.CoinsSpent != userQuest.CoinsSpent)
                entry.Property(x => x.CoinsSpent).IsModified = true;

            if (dbUserQuest.VoiceUptimeEarned != userQuest.VoiceUptimeEarned)
                entry.Property(x => x.VoiceUptimeEarned).IsModified = true;

            if (dbUserQuest.GiftedDeveloper != userQuest.GiftedDeveloper)
                entry.Property(x => x.GiftedDeveloper).IsModified = true;

            if (dbUserQuest.GiftedModerator != userQuest.GiftedModerator)
                entry.Property(x => x.GiftedModerator).IsModified = true;

            if (dbUserQuest.GiftedStreamer != userQuest.GiftedStreamer)
                entry.Property(x => x.GiftedStreamer).IsModified = true;

            if (dbUserQuest.ActivatedBotRespects != userQuest.ActivatedBotRespects)
                entry.Property(x => x.ActivatedBotRespects).IsModified = true;

            if (dbUserQuest.OpenedSphere != userQuest.OpenedSphere)
                entry.Property(x => x.OpenedSphere).IsModified = true;

            if (dbUserQuest.RolesPurchased != userQuest.RolesPurchased)
                entry.Property(x => x.RolesPurchased).IsModified = true;

            await context.SaveChangesAsync();
        }

        public async Task<RiftQuest> GetNextQuestInStage(int stageId, int previousQuestOrder)
        {
            await using var context = new RiftContext();
            return await context.Quests
                .Where(x => x.StageId == stageId && x.Order > previousQuestOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasCompletedStageAsync(ulong userId, int stageId)
        {
            var lastQuest = await GetLastQuestAsync(stageId);

            await using var context = new RiftContext();
            return await context.QuestProgress
                .AnyAsync(x => x.UserId == userId && x.QuestId == lastQuest.Id && x.IsCompleted);
        }
    }
}
