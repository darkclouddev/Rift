using Rift.Configuration;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class RegisterEmbeds
    {
        public static Embed AlreadyHasAcc =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас уже имеется привязанный аккаунт.")
                .Build();

        public static Embed AcceptPending =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Вы уже добавили аккаунт. Ожидаю подтверждения.")
                .Build();

        public static Embed WrongSummonerName(string summonerName, string region)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Не удалось найти игрока {summonerName} на сервере {region}")
                   .Build();
        }

        public static Embed AccountIsUsed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Этот аккаунт уже привязан кем-то другим.")
                .Build();

        public static Embed CodeGenerated(string code)
        {
            return new EmbedBuilder()
                   .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                   .WithDescription($"Используйте код **{code}** для подтверждения вашего аккаунта.\n"
                                    + $"Скопируйте и вставьте его в настройках клиента в раздел \"подтверждение\".")
                   .Build();
        }

        public static Embed UserNotExists =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Пользователь не найден.")
                .Build();

        public static Embed RegistrationSuccessful =
            new EmbedBuilder()
                .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                .WithColor(73, 197, 105)
                .WithDescription($"Ваш игровой аккаунт подтверждён, выдана роль на сервере в соответствии с вашим рангом. Проверьте статистику командой в чат: `!игровой аккаунт`")
                .Build();

        public static Embed RegistrationReward =
            new EmbedBuilder()
                .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                .WithDescription($"Вы получили {Settings.Emote.Chest} 2 за подтверждение игрового аккаунта.")
                .Build();

        public static Embed ChatEmbed(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                   .WithDescription($"Призыватель {sgUser.Mention} подтвердил свой игровой аккаунт.")
                   .Build();
        }
    }
}
