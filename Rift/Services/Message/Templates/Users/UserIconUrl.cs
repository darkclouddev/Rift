using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserIconUrl : TemplateBase
    {
        public UserIconUrl() : base(nameof(UserIconUrl)) {}

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, data.UserId);

            if (user is null)
            {
                TemplateError("No user data found.");
                return message;
            }

            return await ReplaceDataAsync(message, user.GetAvatarUrl());
        }
    }
}
