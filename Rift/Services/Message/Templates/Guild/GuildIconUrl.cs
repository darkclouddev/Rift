using System.Threading.Tasks;

using IonicLib;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Guild
{
    public class GuildIconUrl : TemplateBase
    {
        public GuildIconUrl() : base(nameof(GuildIconUrl))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
            {
                TemplateError($"No guild found with ID {Settings.App.MainGuildId.ToString()}!");
                return Task.FromResult(message);
            }

            return ReplaceDataAsync(message, guild.IconUrl);
        }
    }
}
