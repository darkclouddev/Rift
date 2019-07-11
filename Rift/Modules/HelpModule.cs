using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Util;

using Discord;
using Discord.Commands;

using IonicLib;
using IonicLib.Util;

using Rift.Services.Message;

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
                sb.AppendLine(
                    "**mute** - накладывает мут на упомянутый аккаунт. Снимается автоматически по истечении указанного времени.");
                sb.AppendLine(
                    "Доступные модификаторы времени: **s** (секунды), **m** (минуты), **h** (часы), **d** (дни).");
                sb.AppendLine("Комбинации модификаторов не допускается.");
                sb.AppendLine("При накладывании мута на уже замученный аккаунт старый мут заменяется новым.");
                sb.AppendLine();
                sb.AppendLine("__Примеры использования:__");
                sb.AppendLine("Замутить аккаунт на 1 час");
                sb.AppendLine("`!mute @Yasuo 1h \"причина\"`");
                sb.AppendLine();
                sb.AppendLine("Замутить аккаунт на 29 секунд");
                sb.AppendLine("`!mute @Yasuo 29s \"причина\"`");
                sb.AppendLine();
                sb.AppendLine("**unmute** - снимает мут с упомянутого аккаунта. Просто и понятно.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!unmute @Yasuo \"причина\"`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("**warn** - выдаёт предупреждение упомянутому аккаунту.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!warn @Yasuo \"причина\"`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("**kick** - кикает упомянутый аккаунт с сервера.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!kick @Yasuo \"причина\"`");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(
                    $"**ban** - банит упомянутый аккаунт на сервере и удаляет все его сообщения за последние 24 часа.");
                sb.AppendLine();
                sb.AppendLine("__Пример использования:__");
                sb.AppendLine("`!ban @Yasuo \"причина\"`");

                ModHelpMessage = sb.ToString();
            }

            await Context.Channel.SendMessageAsync(ModHelpMessage);
        }

        static RiftEmbed lvlHelpEmbed = null;

        [Command("lvlhelp")]
        [RequireModerator]
        public async Task LvlHelp()
        {
            if (lvlHelpEmbed is null)
            {
                var lvlHelpEmbed = new RiftEmbed()
                    .WithDescription($"Награды за уровни на сервере\n\n"
                                     + $"Проявляйте активность в общем чате и получайте монеты, сундуки и редкие жетоны. С поднятием уровня вам будут открываться дополнительные возможности с ботом и награды будут увеличиваться.\n\n"
                                     + $"Награды за 2 уровень:\n"
                                     + $"Все призыватели получают 100 1");
            }

            await Context.Channel.SendIonicMessageAsync(new IonicMessage(lvlHelpEmbed));
        }
    }
}
