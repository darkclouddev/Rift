using System.Globalization;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserJoinedDate : TemplateBase
    {
        public UserJoinedDate() : base(nameof(UserJoinedDate))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, data.UserId, out var sgUser)
                || sgUser?.JoinedAt is null)
            {
                TemplateError("No user data found.");
                return message;
            }

            return await ReplaceDataAsync(message, sgUser.JoinedAt.Value.UtcDateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }
    }
}
