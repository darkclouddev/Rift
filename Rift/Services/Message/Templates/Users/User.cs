using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class User : TemplateBase
    {
        public User() : base(nameof(User)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (sgUser is null)
            {
                TemplateError("No user data found.");
                return null;
            }

            return ReplaceDataAsync(message, sgUser.ToString());
        }
    }
}
