using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Util;

using Discord;
using Discord.Commands;

namespace Rift.Modules
{
    public class HelpModule : RiftModuleBase
    {
        static string ModHelpMessage { get; set; }
        static string WelcomeMessage { get; set; }

        [Command("команды")]
        [RequireContext(ContextType.Guild)]
        public async Task Help()
        {
            var msg = await RiftBot.GetMessageAsync("help-commands", null);
            await Context.Channel.SendIonicMessageAsync(msg);
        }

        [Command("правила")]
        [RequireContext(ContextType.Guild)]
        public async Task Rules()
        {
            var sb = new StringBuilder()
                     .AppendLine($"1. Текстовый канал <#{Settings.ChannelId.Comms.ToString()}>")
                     .AppendLine("1.1 Любой 18+ контент запрещён. Наказание: блокировка чата 2-4 часа/бан.")
                     .AppendLine("1.2 Запрещено использование ненормативной лексики. Наказание: блокировка чата 2-6 часов.")
                     .AppendLine("1.3 Запрещено оскорблять или провоцировать других призывателей. Наказание: блокировка чата 4-12 часов.")
                     .AppendLine("1.4 Спам, флуд и капс в объемном количестве запрещены. Наказание: предупреждение/блокировка чата 1-3 часа.")
                     .AppendLine("1.5 Реклама любых сторонних ресурсов связанных с бустом, продажей аккаунтов и RP, а так же других серверов запрещены. Наказание: бан.")
                     .AppendLine("1.6 Оскорбления в сторону ваших тиммейтов и сотрудников Riot games в контексте перехода на личности запрещены. Наказание: блокировка чата 2-4 часа.")
                     .AppendLine()
                     .AppendLine($"2. Текстовый канал <#{Settings.ChannelId.Search.ToString()}>")
                     .AppendLine("2.1 Излишнее общение в данном чате не рекомендуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine("2.2 Чат используется только для поиска игроков и любое другое его использование не приветствуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine()
                     .AppendLine($"3. Текстовый канал <#{Settings.ChannelId.Comms.ToString()}>")
                     .AppendLine("3.1 Размещение скриншотов не связанных с League of Legends запрещено. Наказание: предупреждение/блокировка чата 2-4 часа.")
                     .AppendLine("3.2 Обсуждения вне рамок размещенных скриншотов не приветствуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine()
                     .AppendLine("Руководство сервера может проверить все ваши действия и действия модерации, поэтому не пытайтесь ввести нас в заблуждение.")
                     .AppendLine("Обсуждение и тем более осуждение действий модерации не рекомендуется. Все претензии вы можете высказать Гл. Модераторам или самим Модераторам в личных сообщения.")
                     .AppendLine("Модераторы имеют право принимать решения по наказаниям самостоятельно, но рамках выше описанных правил, а вы имеете право их оспаривать, обратившись к Гл.Модераторам.");

            var msg = await RiftBot.GetMessageAsync("help-rules", null);
            await Context.Channel.SendIonicMessageAsync(msg);
        }

        [Command("модерация")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Moderation()
        {
            if (string.IsNullOrEmpty(ModHelpMessage))
            {
                var sb = new StringBuilder();
                sb.AppendLine("__**Список команд**__");
                sb.AppendLine();
                sb.AppendLine("**mute** - накладывает мут на упомянутый аккаунт. Снимается автоматически по истечении указанного времени.");
                sb.AppendLine("Доступные модификаторы времени: **s** (секунды), **m** (минуты), **h** (часы), **d** (дни).");
                sb.AppendLine("Комбинации модификаторов не допускается.");
                sb.AppendLine("При накладывании мута на уже замученный аккаунт старый мут заменяется новым.");
                sb.AppendLine();
                sb.AppendLine("__Примеры использования:__");
                sb.AppendLine("Замутить аккаунт на 1 час");
                sb.AppendLine("`!mute 1h @Yasuo`");
                sb.AppendLine();
                sb.AppendLine("Замутить аккаунт на 29 секунд");
                sb.AppendLine("`!mute 29s @Yasuo`");
                sb.AppendLine();
                sb.AppendLine("**unmute** - снимает мут с упомянутого аккаунта. Просто и понятно.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!unmute @Yasuo`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("**warn** - выдаёт предупреждение упомянутому аккаунту.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!warn @Yasuo`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("**kick** - кикает упомянутый аккаунт с сервера.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!kick @Yasuo`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"**ban** - банит упомянутый аккаунт на сервере и удаляет все его сообщения за последние 24 часа.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!ban @Yasuo`");

                ModHelpMessage = sb.ToString();
            }

            await Context.User.SendMessageAsync(ModHelpMessage);
        }
    }
}
