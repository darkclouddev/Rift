using System;

using IonicLib.Extensions;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    public static class BanhammerEmbeds
    {
        public static Embed Kick(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                .WithDescription($"Призыватель **{sgUser}** был кикнут с сервера.")
                .Build();
        }

        public static Embed KickLog(SocketGuildUser sgUser, string executorName, string reason)
        {
            return new EmbedBuilder()
                .WithColor(255, 0, 0)
                .WithAuthor("Kicked")
                .WithTitle(sgUser.ToString())
                .WithDescription($"\n`{reason}`")
                .AddField("Executor", executorName)
                .WithTimestamp(DateTime.UtcNow)
                .Build();
        }

        public static Embed Ban(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                .WithDescription($"Призыватель **{sgUser}** был заблокирован на сервере.")
                .Build();
        }
        
        public static Embed BanLog(SocketGuildUser sgUser, string executorName, string reason)
        {
            return new EmbedBuilder()
                .WithColor(255, 0, 0)
                .WithAuthor("Banned")
                .WithTitle(sgUser.ToString())
                .WithDescription($"\n`{reason}`")
                .AddField("Executor", executorName)
                .WithTimestamp(DateTime.UtcNow)
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
        
        public static Embed MuteLog(SocketGuildUser sgUser, string executorName, TimeSpan duration, string reason)
        {
            return new EmbedBuilder()
                .WithColor(255, 0, 0)
                .WithAuthor("Muted")
                .WithTitle(sgUser.ToString())
                .WithDescription($"\n`{reason}`")
                .AddField("Executor", executorName, true)
                .AddField("Duration", duration.FormatTimeToString(), true)
                .WithTimestamp(DateTime.UtcNow)
                .Build();
        }
        
        public static Embed UnmuteLog(SocketGuildUser sgUser, string executorName, string reason)
        {
            return new EmbedBuilder()
                .WithColor(0, 255, 0)
                .WithAuthor("Unmuted")
                .WithTitle(sgUser.ToString())
                .WithDescription($"\n`{reason}`")
                .AddField("Executor", executorName)
                .WithTimestamp(DateTime.UtcNow)
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
