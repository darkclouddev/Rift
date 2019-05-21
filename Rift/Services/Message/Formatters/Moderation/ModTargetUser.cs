using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters.Moderation
{
    public class ModTargetUser : FormatterBase
    {
        public ModTargetUser() : base("$modTargetUser") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.Moderation.TargetId);

            if (sgUser is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(ModTargetUser)}\": No user data found.");
                return null;
            }

            return ReplaceData(message, sgUser.ToString());
        }
    }
}
