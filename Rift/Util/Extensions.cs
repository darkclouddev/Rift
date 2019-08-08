using System;
using System.Net;
using System.Threading.Tasks;

using Rift.Services.Message;

using Discord;
using Discord.WebSocket;

namespace Rift.Util
{
    public static class Extensions
    {
        const string ImageFileName = "image.png";

        public static async Task<IUserMessage> SendIonicMessageAsync(this ITextChannel channel, IonicMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (!string.IsNullOrWhiteSpace(message.ImageUrl))
            {
                var request = WebRequest.Create(message.ImageUrl);

                using (var stream = (await request.GetResponseAsync()).GetResponseStream())
                {
                    return await channel.SendFileAsync(stream, ImageFileName, message.Text ?? "", embed: message.Embed);
                }
            }

            return await channel.SendMessageAsync(message.Text ?? "", embed: message.Embed)
                                .ConfigureAwait(false);
        }

        public static async Task<IUserMessage> SendIonicMessageAsync(this IMessageChannel channel, IonicMessage message)
        {
            return await SendIonicMessageAsync((ITextChannel) channel, message);
        }

        public static async Task<IUserMessage> SendIonicMessageAsync(this IUser user, IonicMessage message)
        {
            return await SendIonicMessageAsync(await user.GetOrCreateDMChannelAsync(), message);
        }

        public static string ToLogString(this IUser sgUser)
        {
            return $"[{sgUser}|{sgUser.Id.ToString()}]";
        }

        public static string ToLogString(this SocketGuildUser sgUser)
        {
            return $"[{sgUser}|{sgUser.Id.ToString()}]";
        }
    }
}
