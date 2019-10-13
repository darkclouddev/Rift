using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

using IonicLib;

namespace Rift.Services.Message.Templates.Users
{
    public class UserIconUrl : TemplateBase
    {
        public UserIconUrl() : base(nameof(UserIconUrl))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, data.UserId, out var sgUser))
            {
                TemplateError("No user data found.");
                return message;
            }

            return await ReplaceDataAsync(message, sgUser.GetAvatarUrl());
        }
    }
}
