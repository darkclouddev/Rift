using System.Globalization;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserJoinedDate : FormatterBase
    {
        public UserJoinedDate() : base("$userJoinedDate") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (user?.JoinedAt is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(UserJoinedDate)}\": No user data found.");
                return message;
            }

            return await ReplaceData(message, user.JoinedAt.Value.UtcDateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }
    }
}
