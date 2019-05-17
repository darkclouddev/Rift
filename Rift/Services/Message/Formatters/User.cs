using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters
{
    public class User : FormatterBase
    {
        public User() : base("$user") {}

        public override Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(User)}\": No user data found.");
                return null;
            }

            return ReplaceData(message, sgUser.ToString());
        }
    }
}
