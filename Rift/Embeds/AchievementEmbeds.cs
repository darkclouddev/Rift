using Rift.Configuration;
using Rift.Data.Models;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class AchievementEmbeds
    {
        public static Embed AchievementsEmbed(RiftAchievements achievements)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ваши достижения на сервере")
                   .WithThumbnailUrl(Settings.Thumbnail.Achievement)
                   .WithDescription("Информация и все возможные достижения:\n\n"
                                    + $"{GetAchievementMark(achievements.Write100Messages)} Общительный (100 сообщений)\n"
                                    + $"{GetAchievementMark(achievements.Write1000Messages)} Без личной жизни (1000 сообщений)\n"
                                    + $"{GetAchievementMark(achievements.Reach10Level)} Активный (10 уровень)\n"
                                    + $"{GetAchievementMark(achievements.Reach30Level)} Постоянный (30 уровень)\n"
                                    + $"{GetAchievementMark(achievements.Brag100Times)} Хвастунишка (100 раз похвастаться)\n"
                                    + $"{GetAchievementMark(achievements.Attack200Times)} Линчеватель (200 раз атаковать)\n"
                                    + $"{GetAchievementMark(achievements.OpenSphere)} Нечто классное (открыть одну сферу)\n"
                                    + $"{GetAchievementMark(achievements.Purchase200Items)} Транжира (купить 200 товаров в магазине)\n"
                                    + $"{GetAchievementMark(achievements.Open100Chests)} Золотоискатель (открыть 100 сундуков)\n"
                                    + $"{GetAchievementMark(achievements.Send100Gifts)} Альтруист (подарить 100 подарков)\n"
                                    + $"{GetAchievementMark(achievements.IsDonator)} Спонсор (хоть раз пожертвовать серверу)\n"
                                    + $"{GetAchievementMark(achievements.HasDonatedRole)} Несравненный (купить любую платную роль)\n"
                                    + $"{GetAchievementMark(achievements.GiftToBotKeeper)} Умный ход (подарить один подарок хранителю бота)\n"
                                    + $"{GetAchievementMark(achievements.GiftToModerator)} Покажи себя (подарить один подарок модератору)\n"
                                    + $"{GetAchievementMark(achievements.AttackWise)} Разоритель основателя (напасть на основателя сервера)")
                   .Build();
        }

        static string GetAchievementMark(bool achievement)
        {
            return achievement ? Settings.Emote.MarkedAchievement : Settings.Emote.UnmarkedAchievement;
        }

        public static Embed ChatEmbed(ulong userId, string achievementName)
        {
            return new EmbedBuilder()
                   .WithTitle("Достижение")
                   .WithColor(103, 240, 154)
                   .WithDescription($"Призыватель <@{userId}> получает {Settings.Emote.MarkedAchievement} {achievementName}.")
                   .Build();
        }

        public static Embed DMEmbed(Reward reward, string achievementName)
        {
            return new EmbedBuilder()
                   .WithAuthor($"Достижение", Settings.Emote.MarkedAchievementUrl)
                   .WithColor(103, 240, 154)
                   .WithDescription($"Вы получили {reward.RewardString} за достижение {achievementName}.")
                   .Build();
        }
    }
}
