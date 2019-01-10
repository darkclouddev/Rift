using System;

using IonicLib.Extensions;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class AdminEmbeds
    {
        public static readonly Embed ShutDown =
            new EmbedBuilder().WithDescription($"Основной бот сервера выключен.").Build();

        public static Embed Kick(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель **{sgUser}** был кикнут с сервера.")
                   .Build();
        }

        public static Embed Ban(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель **{sgUser}** был заблокирован на сервере.")
                   .Build();
        }

        public static Embed ChatMute(SocketGuildUser user, string reason)
        {
            return new EmbedBuilder()
                   .WithAuthor("Оповещение")
                   .WithDescription($"Призывателю {user.Mention} выдается блокировка чата.\n"
                                    + $"Комментарий модератора: {reason}")
                   .Build();
        }

        public static Embed DMMute(TimeSpan ts, string reason)
        {
            return new EmbedBuilder()
                   .WithAuthor("Оповещение")
                   .WithDescription($"Вам выдана блокировка чата на {ts.FormatTimeToString()}.\n"
                                    + $"Комментарий модератора: `{reason}`")
                   .Build();
        }

        public static Embed Warn(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithAuthor("Оповещение")
                   .WithDescription($"Призывателю {sgUser.Mention} выносится предупреждение.")
                   .Build();
        }
    }
}
