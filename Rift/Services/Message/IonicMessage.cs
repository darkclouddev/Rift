using System;

using Rift.Data.Models;

using Discord;

using Newtonsoft.Json;

namespace Rift.Services.Message
{
    public class IonicMessage
    {
        public string Text { get; }
        public Embed Embed { get; }
        public string ImageUrl { get; }

        public IonicMessage(string text)
        {
            Text = text;
            Embed = null;
            ImageUrl = null;
        }

        public IonicMessage(string text, Embed embed, string imageUrl = null)
        {
            Text = text;
            Embed = embed;
            ImageUrl = imageUrl;
        }

        public IonicMessage(string text, RiftEmbed embed, string imageUrl = null)
        {
            Text = text;
            Embed = embed.ToEmbed();
            ImageUrl = imageUrl;
        }

        public IonicMessage(Embed embed)
        {
            Embed = embed;
            Text = null;
            ImageUrl = null;
        }

        public IonicMessage(RiftEmbed riftEmbed)
        {
            Embed = riftEmbed.ToEmbed();
            Text = null;
            ImageUrl = null;
        }

        public IonicMessage(RiftMessage msg)
        {
            Text = msg.Text;
            ImageUrl = msg.ImageUrl;

            if (msg.Embed is null)
                return;

            try
            {
                Embed = JsonConvert
                        .DeserializeObject<RiftEmbed>(
                            msg.Embed, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore})
                        .ToEmbed();
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error($"Failed to deserialize \"{nameof(RiftMessage)}\"!");
                RiftBot.Log.Error(ex);
                Embed = null;
            }
        }
    }
}
