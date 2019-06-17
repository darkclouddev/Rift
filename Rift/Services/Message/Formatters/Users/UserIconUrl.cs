using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserIconUrl : FormatterBase
    {
        public UserIconUrl() : base("$userIconUrl") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (user is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(UserIconUrl)}\": No user data found.");
                return message;
            }

            return await ReplaceData(message, user.GetAvatarUrl());
        }
    }
}
