using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Moderation
{
    public class ModName : TemplateBase
    {
        public ModName() : base(nameof(ModName)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.Moderation.ModeratorId);

            if (sgUser is null)
            {
                TemplateError("No user data found.");
                return Task.FromResult(message);
            }

            return ReplaceDataAsync(message, sgUser.Username);
        }
    }
}
