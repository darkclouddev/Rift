using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters.Guild
{
    public class GuildName : FormatterBase
    {
        public GuildName() : base("$guildName") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
            {
                RiftBot.Log.Error($"Template \"${nameof(GuildName)}\": No guild found.");
                return Task.FromResult(message);
            }

            return ReplaceData(message, guild.Name);
        }
    }
}
