using System.Collections.Concurrent;
using System.Threading.Tasks;

using Rift.Data.Models;

using Discord.WebSocket;
using IonicLib;

namespace Rift.Services
{
    public class EmoteService
    {
        readonly ConcurrentDictionary<string, Emote> emotes;

        public EmoteService()
        {
            emotes = new ConcurrentDictionary<string, Emote>();
        }

        public Task AddEmotesFromGuild(SocketGuild guild)
        {
            foreach (var emote in guild.Emotes)
            {
                if (!emotes.TryAdd($"$emote{emote.Name}", new Emote(emote.Id, emote.Name, emote.Url)))
                {
                    RiftBot.Log.Error(emotes.ContainsKey(emote.Name)
                        ? $"[{nameof(EmoteService)}] Duplicate emote \"{emote.Name}\" from {guild.Name}, skipping."
                        : $"[{nameof(EmoteService)}] Failed to add emote \"{emote.Name}\" from {guild.Name}.");
                }
            }

            //RiftBot.Log.Info($"{nameof(EmoteService)} Loaded {guild.Emotes.Count.ToString()} emote(s) from {guild.Name}");
            return Task.CompletedTask;
        }

        public Task ReloadEmotesAsync()
        {
            emotes.Clear();

            foreach (var guild in IonicClient.Client.Guilds)
            {
                AddEmotesFromGuild(guild);
            }

            RiftBot.Log.Info($"{nameof(EmoteService)} Loaded {emotes.Count.ToString()} emote(s) from {IonicClient.Client.Guilds.Count.ToString()} guild(s)");
            return Task.CompletedTask;
        }

        const string EmoteUrlPostfix = "Url";

        public RiftMessage FormatMessage(string template, RiftMessage message)
        {
            var replacement = GetReplacement(template);

            if (message.Text != null)
                message.Text = message.Text.Replace(template, replacement);

            if (message.Embed != null)
                message.Embed = message.Embed.Replace(template, replacement);

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
                    RiftBot.Log.Warn(
                        $"[{nameof(EmoteService)}] Emote template \"{template}\" does not exist, skipping.");
                    return template;
                }

                return emote.Url;
            }
            else
            {
                if (!emotes.TryGetValue(template, out var emote))
                {
                    RiftBot.Log.Warn(
                        $"[{nameof(EmoteService)}] Emote template \"{template}\" does not exist, skipping.");
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
            Url = $"https://cdn.discordapp.com/emojis/{Id}.png";
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
