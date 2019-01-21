using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

using Discord;

namespace Rift.Services
{
    public class AnnounceService
    {
#pragma warning disable 169
        static Timer timer;
#pragma warning restore 169
        static readonly TimeSpan cooldown = TimeSpan.FromMinutes(30);

        static readonly List<Embed> embeds = new List<Embed>
        {
            new EmbedBuilder()
                .WithAuthor("Полезная ссылка", Settings.Emote.VkUrl)
                .WithDescription($"Присоединяйтесь к нашей беседе вконтакте:"
                                 + $"https://vk.me/join/AJQ1d6OC4wNM1SkhKhzJO_cZ")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Полезная ссылка", Settings.Emote.VkUrl)
                .WithDescription($"Наша группа вконтакте: https://vk.com/lolru")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Проверяйте свой инвентарь с помощью команды `!ивентарь` в чат.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Открывайте все сундуки с помощью команды `!открыть все сундуки`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Напишите в чат `!команды` и узнайте основные команды бота.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"В текстовом канале <#{Settings.ChannelId.Information.ToString()}> можно узнать все о ботах сервера.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"В текстовом канале <#{Settings.ChannelId.Search.ToString()}> ищите тиммейтов.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Пишите каждый день в чат и получайте ежедневные награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Хвастайтесь своими играми в общем чате и получайте монеты.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Убивайте лесных монстров в общем чате и получайте награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Участвуйте в розыгрышах и получайте подарки: <#{Settings.ChannelId.Giveaways.ToString()}>")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Открывайте сферы и получайте роли на 30 дней.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Открывайте редкие капсулы и получайте роли на 30 дней.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Призыватели с ролью **легендарные** получают уникальные награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Призыватели с ролью **абсолютные** получают уникальные награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Подтвердите свой игровой аккаунт <#{Settings.ChannelId.Confirmation.ToString()}> и получите награду.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Статистика вашего игрового аккаунта обновляется автоматически.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"На втором уровне всем призывателям открывается доступ к атакам.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Станьте **хранителем жетонов** и получайте жетоны каждый день.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Проверяйте свои достижения с помощью команды `!достижения` в чат.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Получайте за каждый уровень сундуки, не забывайте их открывать.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Выполняйте достижения и получайте уникальные награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Радуйте других игроков сервера подарками, напишите в чат `!подарки`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Посмотрите товары в магазине с помощью команды в чат `!магазин`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Узнайте все об атаках с помощью команды в чат `!атаки`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Узнайте о бонусах больше в текстовом канале <#{Settings.ChannelId.Information.ToString()}>")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Отслеживайте активные бонусы в профиле `!профиль`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Отслеживайте количество предметов в инвентаре `!инвентарь`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Все временные роли отображаются в профиле `!профиль`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Проверяйте статистику ваших действий в системе командой `!статистика`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Проверяйте свой игровой профиль командой `!игровой профиль`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Проверяйте доступные роли на сервере командой `!роли`.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Все платные роли отображаются справа в списке пользователей.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Покупайте наборы в <#{Settings.ChannelId.Donate.ToString()}> и получайте подарки.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Поддерживайте сервер и получайте уникальные роли: <#{Settings.ChannelId.Donate.ToString()}>")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Все платные роли получают дополнительные награды на уровнях.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"На некоторых уровнях могут выпадать уникальные награды.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Иногда в чате появляются миньоны, убивайте их и получайте награду.")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Узнайте подробнее о нашем клубе в игре в текстовом канале <#{Settings.ChannelId.Clubs.ToString()}>")
                .Build(),
            new EmbedBuilder()
                .WithAuthor("Подсказка", Settings.Emote.QuestionMarkUrl)
                .WithDescription($"Каждый день в чате появляются лесные монстры, убивайте их.")
                .Build(),
        };

        public AnnounceService()
        {
            //timer = new Timer(async delegate { await Announce_Callback(); }, null, cooldown, cooldown);
        }

        async Task Announce_Callback()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendEmbedAsync(embeds.Random());
        }
    }
}
