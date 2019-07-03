using System.Globalization;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserJoinedDate : TemplateBase
    {
        public UserJoinedDate() : base(nameof(UserJoinedDate)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (user?.JoinedAt is null)
            {
                TemplateError("No user data found.");
                return message;
            }

            return await ReplaceData(message, user.JoinedAt.Value.UtcDateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }
    }
}
