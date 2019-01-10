using System;
using System.Text;
using System.Threading.Tasks;

using Rift.Embeds;
using Rift.Configuration;
using Rift.Preconditions;

using IonicLib.Util;

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
            await Context.User.SendEmbedAsync(HelpEmbeds.Commands);
        }

        [Command("правила")]
        [RequireContext(ContextType.Guild)]
        public async Task Rules()
        {
            var sb = new StringBuilder()
                     .AppendLine($"1. Текстовый канал <#{Settings.ChannelId.Chat}>")
                     .AppendLine("1.1 Любой 18+ контент запрещён. Наказание: блокировка чата 2-4 часа/бан.")
                     .AppendLine("1.2 Запрещено использование ненормативной лексики. Наказание: блокировка чата 2-6 часов.")
                     .AppendLine("1.3 Запрещено оскорблять или провоцировать других призывателей. Наказание: блокировка чата 4-12 часов.")
                     .AppendLine("1.4 Спам, флуд и капс в объемном количестве запрещены. Наказание: предупреждение/блокировка чата 1-3 часа.")
                     .AppendLine("1.5 Реклама любых сторонних ресурсов связанных с бустом, продажей аккаунтов и RP, а так же других серверов запрещены. Наказание: бан.")
                     .AppendLine("1.6 Оскорбления в сторону ваших тиммейтов и сотрудников Riot games в контексте перехода на личности запрещены. Наказание: блокировка чата 2-4 часа.")
                     .AppendLine()
                     .AppendLine($"2. Текстовый канал <#{Settings.ChannelId.Search}>")
                     .AppendLine("2.1 Излишнее общение в данном чате не рекомендуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine("2.2 Чат используется только для поиска игроков и любое другое его использование не приветствуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine()
                     .AppendLine($"3. Текстовый канал <#{Settings.ChannelId.Screenshots}>")
                     .AppendLine("3.1 Размещение скриншотов не связанных с League of Legends запрещено. Наказание: предупреждение/блокировка чата 2-4 часа.")
                     .AppendLine("3.2 Обсуждения вне рамок размещенных скриншотов не приветствуется. Наказание: предупреждение/блокировка чата 1-2 часа.")
                     .AppendLine()
                     .AppendLine("Руководство сервера может проверить все ваши действия и действия модерации, поэтому не пытайтесь ввести нас в заблуждение.")
                     .AppendLine("Обсуждение и тем более осуждение действий модерации не рекомендуется. Все претензии вы можете высказать Гл. Модераторам или самим Модераторам в личных сообщения.")
                     .AppendLine("Модераторы имеют право принимать решения по наказаниям самостоятельно, но рамках выше описанных правил, а вы имеете право их оспаривать, обратившись к Гл.Модераторам.");

            await Context.User.SendEmbedAsync(HelpEmbeds.Rules);
            await Context.User.SendMessageAsync(text: sb.ToString());
        }

        [Command("модерация")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task Moderation()
        {
            if (String.IsNullOrEmpty(ModHelpMessage))
            {
                var sb = new StringBuilder();
                sb.AppendLine(Format.Underline(Format.Bold("Список команд")));
                sb.AppendLine();
                sb.AppendLine($"{Format.Bold("mute")} - накладывает мут на упомянутый аккаунт. Снимается автоматически по истечении указанного времени.");
                sb.AppendLine($"Доступные модификаторы времени: {Format.Bold("s")} (секунды), {Format.Bold("m")} (минуты), {Format.Bold("h")} (часы), {Format.Bold("d")} (дни).");
                sb.AppendLine("Комбинации модификаторов не допускается.");
                sb.AppendLine("При накладывании мута на уже замученный аккаунт старый мут заменяется новым.");
                sb.AppendLine();
                sb.AppendLine(Format.Underline("Примеры использования:"));
                sb.AppendLine("Замутить аккаунт на 1 час");
                sb.AppendLine(Format.Code($"!mute 1h @Yasuo"));
                sb.AppendLine();
                sb.AppendLine("Замутить аккаунт на 29 секунд");
                sb.AppendLine(Format.Code($"!mute 29s @Yasuo"));
                sb.AppendLine();
                sb.AppendLine($"{Format.Bold("unmute")} - снимает мут с упомянутого аккаунта. Просто и понятно.");
                sb.AppendLine();
                sb.AppendLine(Format.Underline("Пример использования:"));
                sb.AppendLine(Format.Code($"!unmute @Yasuo"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"{Format.Bold("warn")} - выдаёт предупреждение упомянутому аккаунту.");
                sb.AppendLine();
                sb.AppendLine(Format.Underline("Пример использования:"));
                sb.AppendLine(Format.Code($"!warn @Yasuo"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"{Format.Bold("kick")} - кикает упомянутый аккаунт с сервера.");
                sb.AppendLine();
                sb.AppendLine(Format.Underline("Пример использования:"));
                sb.AppendLine(Format.Code($"!kick @Yasuo"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"{Format.Bold("ban")} - банит упомянутый аккаунт на сервере и удаляет все его сообщения за последние 24 часа.");
                sb.AppendLine();
                sb.AppendLine(Format.Underline("Пример использования:"));
                sb.AppendLine(Format.Code($"!ban @Yasuo"));

                ModHelpMessage = sb.ToString();
            }

            await Context.User.SendMessageAsync(ModHelpMessage);
        }
    }
}
