using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Moderation
{
    public class ModTargetUser : TemplateBase
    {
        public ModTargetUser() : base(nameof(ModTargetUser)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.Moderation.TargetId);

            if (sgUser is null)
            {
                TemplateError("No user data found.");
                return null;
            }

            return ReplaceData(message, sgUser.ToString());
        }
    }
}
