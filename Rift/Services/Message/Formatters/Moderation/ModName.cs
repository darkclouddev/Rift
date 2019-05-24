using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters.Moderation
{
    public class ModName : FormatterBase
    {
        public ModName() : base("$modName") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.Moderation.ModeratorId);

            if (sgUser is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(ModName)}\": No user data found.");
                return Task.FromResult(message);
            }

            return ReplaceData(message, sgUser.ToString());
        }
    }
}
