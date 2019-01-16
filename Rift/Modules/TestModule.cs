using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services;
using Rift.Preconditions;

using IonicLib;
using IonicLib.Preconditions;
using IonicLib.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Humanizer;

namespace Rift.Modules
{
    [Group("test")]
    [Alias("тест")]
    public class TestModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RoleService roleService;
        readonly MessageService messageService;

        readonly DatabaseService databaseService;

        //readonly DonateService donateService;
        readonly EventService eventService;

        public TestModule(EconomyService economyService,
                          RoleService roleService,
                          MessageService messageService,
                          DatabaseService databaseService,

                          //DonateService donateService,
                          EventService eventService)
        {
            this.economyService = economyService;
            this.roleService = roleService;
            this.messageService = messageService;
            this.databaseService = databaseService;

            //this.donateService = donateService;
            this.eventService = eventService;
        }

        string GetUserNameById(ulong userId)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            return user is null ? "-" : RemoveNonAlphanumeric(user.Username);
        }

        string RemoveNonAlphanumeric(string input)
        {
            char[] symbols = input.Where(x => (char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == '-')).ToArray();

            return new string(symbols);
        }

        static Random random = new Random();

        [Command("since")]
        [RateLimit(1, 10, Measure.Minutes, RateLimitFlags.NoLimitForAdmins)]
        [RequireContext(ContextType.Guild)]
        public async Task Since()
        {
            await ReplyAsync($"{Context.User.Mention} на сервере с"
                             + $" {((SocketGuildUser) Context.User).JoinedAt.Value.LocalDateTime.Humanize()}");
        }

        [Command("baron")]
        [RequireDeveloper]
        public async Task Baron()
        {
            await eventService.StartEvent(EventType.Baron);
        }

        [Command("horse")]
        [RequireDeveloper]
        public async Task Horse()
        {
            var url =
                "http://www.merlinsltd.com/WebRoot/StoreLGB/Shops/62030553/54C2/CD1F/F497/BF84/54D1/C0A8/2ABB/1A80/mask_horse_brown.png";

            var request = WebRequest.Create(url);

            using (var stream = (await request.GetResponseAsync()).GetResponseStream())
            {
                await Context.Channel.SendFileAsync(stream, "horse.png");
            }
        }

        [Command("clubs")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Clubs()
        {
            var message = new StringBuilder()
                          .AppendLine($"Перед вступлением в клуб вы должны ознакомиться с нашими правилами и требованиями, и соблюдать их.")
                          .AppendLine()
                          .AppendLine($"Участие в клубе необходимо подтвердить на сайте https://clubs.ru.leagueoflegends.com/new в день вступления.")
                          .AppendLine($"Каждый сезон клубов необходимо подтверждать повторно, не забывайте!")
                          .AppendLine()
                          .AppendLine($"**Правила нашего клуба:**")
                          .AppendLine($"• Обязательное наличие метки клуба.")
                          .AppendLine($"• Хороший онлайн. Желание играть совместные игры с участниками клуба.")
                          .AppendLine($"• Во время игры необходимо находиться в голосовых каналах на данном дискорд сервере.")
                          .AppendLine($"• Общительность и адекватность (никакого флейма в сторону противников и участников клуба быть не должно).")
                          .ToString();

            var embed1 = new EmbedBuilder()
                         .WithAuthor("Важная информация о клубе")
                         .WithDescription($"- Для вступления необходимо иметь минимум {Settings.Emote.RankGold} золото\n"
                                          + $"- Игроки клуба обязаны фармить 400 + очков за этап(неделю).\n\n"
                                          + $"Связаться с офицерами клуба: <@231687934242324481>, <@249345218866970624>.")
                         .Build();

            var embed2 = new EmbedBuilder()
                         .WithDescription($"Подать заявку для вступления в клуб: https://vk.cc/7MeBRS")
                         .Build();

            //post

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/406792631570661409/493939632397090846/dca0665f3e2f96d0.png");

            await ReplyAsync(message);

            await Context.Channel.SendEmbedAsync(embed1);
            await Context.Channel.SendEmbedAsync(embed2);
        }

        [Command("update")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Update()
        {
            var message = new StringBuilder()
                          .AppendLine($"Используйте всю мощь <@404611580819537920> с обновленной системой, узнайте подробнее: <#{Settings.ChannelId.Information}>")
                          .AppendLine($"Подтвердить игровой аккаунт в канале <#{Settings.ChannelId.Confirmation}> - это первый шаг к тому, чтобы стать частью сообщества нашего голосового сервера.")
                          .AppendLine($"Но рост нашего сообщества требует немного больше возможностей от данного бота. Скоро мы серьезно затронем систему, которая отвечает за взаимодействие игрока и игры на сервере.")
                          .AppendLine()
                          .AppendLine($"**Узнайте важную информацию:**")
                          .AppendLine($"• Обновленная система бота автоматически изменила количество предметов в инвентарях пользователей сервера для уравновешивания экономики.")
                          .AppendLine($"• На днях пользователям придут подарки, чьи инвентари были затронуты ботом.")
                          .AppendLine($"• Через некоторое время включится наш второй бот <@357607386732691457>.")
                          .AppendLine()
                          .AppendLine($"Если вы заметили ошибки в тексте или же баг в системе, то сообщите команде сервера. (гл. модераторы, модераторы, хранители ботов).")
                          .ToString();

            //post

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/387007245520732162/494276862546018327/RiftBot.png");

            await ReplyAsync("Призыватели, @everyone.");

            await ReplyAsync(message);
        }

        [Command("welcome")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Welcome()
        {
            var message = new StringBuilder()
                          .AppendLine($"Добро пожаловать на голосовой сервер по League of legends.")
                          .AppendLine($"Здесь вы  всегда сможете найти себе напарника для игры - не важно, будь то ранговый или обычный матч, даже игра с ботами.")
                          .AppendLine("Найдя тиммейтов, вы сможете легко и быстро присоединиться к любому доступному голосовому каналу.Объединяйтесь, играйте, побеждайте!")
                          .AppendLine()
                          .AppendLine($"**Основные каналы:**")
                          .AppendLine($"**1**. <#{Settings.ChannelId.Confirmation}> для привязки вашего аккаунта в игре к серверу.")
                          .AppendLine($"**2**. <#{Settings.ChannelId.Search}> поможет вам найти напарников в обычные, арамы или ранговые игры.")
                          .AppendLine($"**3**. <#{Settings.ChannelId.Clubs}> для желающих присоединиться к сражению за сундуки и место клуба в сезоне.")
                          .AppendLine($"**4**. <#{Settings.ChannelId.Chat}> позволит вам пообщаться с другими участниками сервера, познакомиться и получить первые награды за активность.")
                          .AppendLine($"**5**. <#{Settings.ChannelId.Information}> существует для того, чтобы вы всегда могли прочесть о системе сервера и ознакомиться с основными командами ботов.")
                          .AppendLine()
                          .AppendLine($"**Правила поведения на сервере:**")
                          .AppendLine($"• Запрещено неуважительное отношение к участникам сервера (сексизм, расизм, травля, провокация).")
                          .AppendLine($"• Запрещено использование ненормативной лексики, спама, флуда, распространение контента 18+.")
                          .AppendLine($"• Запрещено рекламирование и распространение ссылок на сторонние проекты и сервера.")
                          .AppendLine($"• Запрещена покупка/продажа буста, аккаунтов.")
                          .ToString();

            var embed = new EmbedBuilder()
                        .WithDescription($"Приглашайте друзей по данной ссылке: https://discord.gg/lolru\n"
                                         + $"Создавайте приглашения на игровые и другие каналы.")
                        .Build();

            //post

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/387007083997822996/447717266826461184/5.png");

            await ReplyAsync(message);

            await Context.Channel.SendEmbedAsync(embed);
        }

        [Command("info")]
        [RequireDeveloper]
        public async Task Info()
        {
            var message = new StringBuilder()
                          .AppendLine($"Получайте **ед. опыта** за активность на нашем сервере, поднимайте уровень и зарабатывайте награды. Необходимо написать `!команды` в чат, чтобы узнать основные команды бота.")
                          .AppendLine($"На сервере присутствует собственная система прогресса с **монетами** (основная валюта), **жетонами** (редкая валюта), **сундуками**, **сферами** и **капсулами**.")
                          .AppendLine()
                          .AppendLine($"Подтвердите свой игровой аккаунт в канале <#{Settings.ChannelId.Confirmation}> и получите награду. Начните общаться в общем чате <#{Settings.ChannelId.Chat}>, чтобы получить первую награду за активность.")
                          .AppendLine()
                          .AppendLine($"**1.** После того, как вы получили {Settings.Emote.Chest} сундуки за активность, откройте свой первый сундук командой в чате `!открыть сундук`.")
                          .AppendLine($"**2.** Проверьте свой {Settings.Emote.Profile} профиль командой `!профиль`, в нём отображается уровень, статистика уровня, активные бонусы, часы в голосовых каналах, ваши роли и пожертвования.")
                          .AppendLine($"**3.** После этого вам стоит посмотреть в свой {Settings.Emote.Inventory} инвентарь командой `!инвентарь`, в нем отображаются ваши монеты, жетоны, бонусы, билеты, сферы и капсулы.")
                          .AppendLine($"**4.** Не забудьте зайти в {Settings.Emote.Store} магазин сервера по команде в чат `!магазин` и потратить свои монеты, жетоны на покупку сундуков, бонусов, билетов, сфер с ролями.")
                          .AppendLine($"**5.** На втором уровне вам откроется доступ к {Settings.Emote.Attack} атакам, команда в чат `!атаки`. Воруйте монеты, блокируйте чат призывателям и защищайтесь.")
                          .AppendLine($"**6.** Узнайте всё о {Settings.Emote.Gifts} подарках на сервере с помощью команды в чат `!подарки`. Отправляйте подарки и радуйте друзей или других призывателей сервера.")
                          .AppendLine($"**7.** После того, как вы все проверили, вам стоит разобраться с {Settings.Emote.MarkedAchievement} достижениями. Узнайте все о достижениях командой в чат `!достижения`.")
                          .AppendLine()
                          .AppendLine($"**Способы получения монет, жетонов и сундуков:**")
                          .ToString();

            var message2 = new StringBuilder()
                           .AppendLine()
                           .AppendLine($"• За каждый уровень получайте {Settings.Emote.Chest} сундуки, не забывайте их открывать.")
                           .AppendLine($"• Выполняйте достижения и получайте уникальные награды и даже {Settings.Emote.Token} редкие жетоны.")
                           .AppendLine($"• Общайтесь в чате больше, каждые 24 часа у вас есть возможность получать {Settings.Emote.Coin} монеты и {Settings.Emote.Experience} ед. опыта за первые сообщения дня.")
                           .AppendLine($"• Каждый день в чате появляются {Settings.Emote.Baron} {Settings.Emote.Drake} {Settings.Emote.Razorfins} {Settings.Emote.Wolves} {Settings.Emote.Gromp} лесные монстры, убивайте их и получайте {Settings.Emote.Coin} монеты, {Settings.Emote.Chest} сундуки.")
                           .AppendLine($"• После подтверждения вашего игрового аккаунта, вы можете хвастаться в чате своими играми каждые 6 часов и получать за это {Settings.Emote.Coin} монеты.")
                           .AppendLine()
                           .AppendLine($"**Сферы, сундуки, капсулы и билеты:**")
                           .ToString();

            var message3 = new StringBuilder()
                           .AppendLine($"Из одной {Settings.Emote.Sphere} сферы можно гарантированно получить {Settings.Emote.Coin} монеты, {Settings.Emote.UsualTickets} обычные билеты, {Settings.Emote.RareTickets} редкие билеты, {Settings.Emote.PowerupDoubleExperience} двойной опыт, {Settings.Emote.BotRespect} уважение ботов.")
                           .AppendLine($"Из {Settings.Emote.Chest} сундука выпадают гарантированно монеты, но также может выпасть {Settings.Emote.UsualTickets} обычный билет, {Settings.Emote.PowerupDoubleExperience} двойной опыт и {Settings.Emote.BotRespect} уважение ботов.")
                           .AppendLine($"А из одной {Settings.Emote.Capsule} капсулы можно получить {Settings.Emote.Coin} монеты, {Settings.Emote.Token} жетоны, {Settings.Emote.PowerupDoubleExperience} двойной опыт, {Settings.Emote.UsualTickets} обычные билеты, {Settings.Emote.RareTickets} редкие билеты и уникальную роль на 60 дней.")
                           .AppendLine()
                           .AppendLine($"Есть два типа билетов и розыгрышей: {Settings.Emote.UsualTickets} обычные билеты используются для обычных розыгрышей, {Settings.Emote.RareTickets} редкие билеты используются для особых розыгрышей.")
                           .AppendLine($"Как только начнется розыгрыш, бот автоматически использует ваш билет и заберет из инвентаря. Проверяйте текстовый канал <#{Settings.ChannelId.Giveaways}>.")
                           .AppendLine()
                           .AppendLine($"**Бонусы и их возможности:**")
                           .ToString();

            var message4 = new StringBuilder()
                           .AppendLine($"Все бонусы активируются с помощью команд:")
                           .AppendLine()
                           .AppendLine($"• Бонус {Settings.Emote.PowerupDoubleExperience} двойной опыт позволяет получать в два раза больше ед. опыта за сообщения.")
                           .AppendLine($"• Бонус {Settings.Emote.BotRespect} уважение ботов поможет подружиться с <@357607386732691457> ботом, а дальше и подарки могут быть.")
                           .AppendLine()
                           .AppendLine($"Команды для активации бонусов: {Settings.Emote.PowerupDoubleExperience} `!активировать двойной опыт`, {Settings.Emote.BotRespect} `!активировать уважение ботов`.")
                           .ToString();

            //post

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/394280800788545548/495241566235918347/1.png");

            await ReplyAsync(message);
            await ReplyAsync(message2);
            await ReplyAsync(message3);
            await ReplyAsync(message4);
        }

        [Command("donate")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Donate()
        {
            var message1 = new StringBuilder()
                           .AppendLine($"На сервере функционирует особая система доната, которая позволит любому желающему внести пожертвование и, в благодарность, получить особые роли и подарки.")
                           .AppendLine($"Все пользователи с платные ролями могут ставить {Settings.Emote.Pogchamp} эмоции под сообщения в общем чате.")
                           .AppendLine()
                           .AppendLine($"Узнайте подробнее о возможностях с платными ролями с помощью команды в чат `!платные роли`.")
                           .AppendLine()
                           .AppendLine($"Набор \"Хранитель жетонов\". Стоимость набора: **200 рублей**.")
                           .AppendLine($"Вы получите уникальную роль {Settings.Emote.Keeper} Хранители жетонов (15 дней) {Settings.Emote.Coin} 10000 {Settings.Emote.Sphere} 1 и дополнительные возможности.")
                           .AppendLine()
                           .AppendLine($"Набор \"Легендарный\". Стоимость набора: **500 рублей**.")
                           .AppendLine($"Вы получите уникальную роль {Settings.Emote.Legendary} Легендарные (навсегда) {Settings.Emote.Coin} 20000 {Settings.Emote.Capsule} 1 и дополнительные возможности.")
                           .AppendLine()
                           .AppendLine($"Набор \"Абсолютный\". Стоимость набора: **3000 рублей**.")
                           .AppendLine($"Вы получите уникальную роль {Settings.Emote.Absolute} Абсолютные (навсегда) {Settings.Emote.Coin} 30000 {Settings.Emote.Capsule} 2 и дополнительные возможности.")
                           .AppendLine()
                           .AppendLine("Перейдите по ссылке ниже для совершения пожертвования или покупки набора.")
                           .ToString();

            var embed1 = new EmbedBuilder()
                         .WithDescription($"Ссылка для покупки наборов: http://donatepay.ru/d/lolru")
                         .Build();

            var message2 = new StringBuilder()
                           .AppendLine("В поле ваше имя укажите свой никнейм и тег (пример: Discord#0001). Важно, чтобы в вашем никнейме не было символов (#,%,!,*,$ и т.д.)")
                           .ToString();

            //post

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/406791810866741269/494826449455546389/bf19d089859c4608.png");

            await ReplyAsync(message1);
            await Context.Channel.SendEmbedAsync(embed1);
            await ReplyAsync(message2);

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/406791810866741269/495262171031142411/image.png");
        }

        [Command("rank")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Rank()
        {
            var sb = new StringBuilder()
                     .AppendLine($"Призыватели, для регистрации вашего игрового аккаунта в нашей системе, вам нужно написать в текстовый канал <#{Settings.ChannelId.Chat}> одну из команд ниже.")
                     .AppendLine()
                     .AppendLine($"Используйте команду `!регистрация RU ваш_никнейм` в чате для привязки аккаунта c русского сервера.")
                     .AppendLine($"Используйте команду `!регистрация EUW ваш_никнейм` в чате для привязки аккаунта c западного сервера.")
                     .AppendLine()
                     .AppendLine($"Если у вас возникли проблемы с регистрацией или же вы решили отвязать игровой аккаунт от нашего сервера, вам необходимо обратиться к модераторам.")
                     .AppendLine()
                     .AppendLine($"1. Основной бот сервера <@404611580819537920> отправит вам в личные сообщения код.")
                     .AppendLine($"2. Скопируйте и вставьте код в поле для подтверждения (как указано на скриншотах ниже).")
                     .AppendLine($"3. Через некоторое время вы получите сообщение в личные сообщения о том, что ваш игровой аккаунт был подтвержден.")
                     .ToString();

            var sb2 = new StringBuilder()
                      .AppendLine($"Вам мгновенно будет выдана роль с соответствующим рангом и, конечно, награда.")
                      .AppendLine()
                      .AppendLine($"Команда `!игровой профиль` позволит вам в любой момент проверить игровую статистику вашего аккаунта. Бот автоматически обновляет ваши данные об аккаунте. ")
                      .AppendLine($"Хвастайтесь каждые 6 часов своими играми при помощи команды `!похвастаться`, бот выбирает случайную игру среди 20 последних матчей, а вы получаете за это {Settings.Emote.Coin} монеты.");

            //post

            await ReplyAsync(sb);

            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/488391413860532245/493576287810617344/1.png");
            await
                Context.Channel
                       .SendImageAsync("https://cdn.discordapp.com/attachments/488391413860532245/493576301123469324/2.png");

            await ReplyAsync(sb2.ToString());
        }

        [Command("exp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task Exp(uint level)
        {
            await ReplyAsync($"Lv {level}: {EconomyService.GetExpForLevel(level)} XP");
        }

        [Command("search")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Search()
        {
            var message1 = new StringBuilder()
                           .AppendLine($"Текстовый канал для поиска напарников в обычные, ранговые игры, флексы или арамы. Переписки не по теме запрещены, они будут удаляться.")
                           .AppendLine($"Приятных вам игр, сражайтесь с честью.")
                           .ToString();

            await ReplyAsync(message1);
        }

        [Command("clash")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task Clash()
        {
            var message1 = new StringBuilder()
                           .AppendLine($"Данный текстовый канал создан специально для облегчения процесса поиска команд и игроков в игровой режим **Clash** русского сервера. Убедительно просим вас оставлять свои анкеты, используя представленную ниже форму.")
                           .AppendLine()
                           .AppendLine($"1. **Имя призывателя**")
                           .AppendLine($"2. **Лига**")
                           .AppendLine($"3. **Роль**")
                           .AppendLine($"4. **Кого ищу**")
                           .AppendLine()
                           .AppendLine($"Сообщения не по теме или содержащие ненормативную лексику будут удаляться.")
                           .AppendLine($"Приятных вам игр, сражайтесь с честью.")
                           .ToString();

            await ReplyAsync(message1);
        }
    }
}
