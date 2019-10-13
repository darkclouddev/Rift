using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Moderation
{
    public class ModName : TemplateBase
    {
        public ModName() : base(nameof(ModName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (IonicHelper.GetGuildUserById(Settings.App.MainGuildId, data.Moderation.ModeratorId, out var sgUser))
            {
                TemplateError("No user data found.");
                return Task.FromResult(message);
            }

            return ReplaceDataAsync(message, sgUser.Username);
        }
    }
}
