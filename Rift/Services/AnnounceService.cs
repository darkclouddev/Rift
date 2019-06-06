using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;

using Discord;
using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

namespace Rift.Services
{
    public class AnnounceService
    {
#pragma warning disable 169
        static Timer timer;
#pragma warning restore 169
        static readonly TimeSpan cooldown = TimeSpan.FromMinutes(30);

        static readonly List<Embed> embeds = new List<Embed>
        {
        };

        public AnnounceService()
        {
            //timer = new Timer(async delegate { await Announce_Callback(); }, null, cooldown, cooldown);
        }

        async Task Announce_Callback()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            await channel.SendEmbedAsync(embeds.Random());
        }
    }
}
