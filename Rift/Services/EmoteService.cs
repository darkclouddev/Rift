using System.Collections.Concurrent;
using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Services.Interfaces;

using Discord.WebSocket;

using IonicLib;

namespace Rift.Services
{
    public class EmoteService : IEmoteService
    {
        readonly ConcurrentDictionary<string, Emote> emotes;

        public EmoteService()
        {
            emotes = new ConcurrentDictionary<string, Emote>();
        }

        public async Task AddEmotesFromGuild(SocketGuild guild)
        {
            foreach (var emote in guild.Emotes)
                if (!emotes.TryAdd($"$emote{emote.Name}", new Emote(emote.Id, emote.Name, emote.Url)))
                {
                    if (emotes.ContainsKey(emote.Name))
                    {
                        var msg = $"Duplicate emote \"{emote.Name}\" from {guild.Name}, skipping.";

                        await RiftBot.SendMessageToAdmins(msg);
                        RiftBot.Log.Error($"[{nameof(EmoteService)}] {msg}.");
                    }
                    else
                    {
                        var msg = $"Failed to add emote \"{emote.Name}\" from {guild.Name}.";

                        await RiftBot.SendMessageToAdmins(msg);
                        RiftBot.Log.Error($"[{nameof(EmoteService)}] {msg}");
                    }
                }
        }

        public async Task ReloadEmotesAsync()
        {
            emotes.Clear();

            foreach (var guild in IonicHelper.Client.Guilds)
                await AddEmotesFromGuild(guild);

            RiftBot.Log.Information($"{nameof(EmoteService)} Loaded {emotes.Count.ToString()} emote(s) from {IonicHelper.Client.Guilds.Count.ToString()} guild(s)");
        }

        const string EmoteUrlPostfix = "Url";

        public RiftMessage FormatMessage(string template, RiftMessage message)
        {
            var replacement = GetReplacement(template);

            if (message.Text != null)
                message.Text = message.Text.Replace(template, replacement);

            if (message.EmbedJson != null)
                message.EmbedJson = message.EmbedJson.Replace(template, replacement);

            return message;
        }

        public string GetEmoteString(string template)
        {
            return GetReplacement(template);
        }

        string GetReplacement(string template)
        {
            if (template.EndsWith(EmoteUrlPostfix))
            {
                var urlTemplate = template.Substring(0, template.Length - 3);

                if (!emotes.TryGetValue(urlTemplate, out var emote))
                {
                    RiftBot.Log.Warning($"[{nameof(EmoteService)}] Emote template \"{template}\" does not exist, skipping.");
                    return template;
                }

                return emote.Url;
            }
            else
            {
                if (!emotes.TryGetValue(template, out var emote))
                {
                    RiftBot.Log.Warning($"[{nameof(EmoteService)}] Emote template \"{template}\" does not exist, skipping.");
                    return template;
                }

                return emote.ToString();
            }
        }
    }

    public class Emote
    {
        public ulong Id { get; }
        public string Name { get; }
        public string Url { get; }

        public Emote(ulong id, string name)
        {
            Id = id;
            Name = name;
            Url = $"https://cdn.discordapp.com/emojis/{Id.ToString()}.png";
        }

        public Emote(ulong id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
        }

        public override string ToString()
        {
            return $"<:{Name}:{Id.ToString()}>";
        }
    }
}
