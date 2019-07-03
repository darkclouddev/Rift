using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserName : TemplateBase
    {
        public UserName() : base(nameof(UserName)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (sgUser is null)
            {
                TemplateError("No user data found.");
                return null;
            }

            return ReplaceData(message, sgUser.Username);
        }
    }
}
