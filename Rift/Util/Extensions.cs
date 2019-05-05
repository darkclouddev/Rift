using System;
using System.Net;
using System.Threading.Tasks;

using Rift.Services.Message;

using Discord;

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
    }
}
