using System.Globalization;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserCreationDate : TemplateBase
    {
        public UserCreationDate() : base(nameof(UserCreationDate))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, data.UserId, out var sgUser))
            {
                TemplateError("No user data found.");
                return message;
            }

            return await ReplaceDataAsync(
                message, sgUser.CreatedAt.UtcDateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }
    }
}
