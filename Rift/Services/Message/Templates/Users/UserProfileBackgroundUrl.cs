using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserProfileBackgroundUrl : TemplateBase
    {
        public UserProfileBackgroundUrl() : base(nameof(UserProfileBackgroundUrl))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var dbUser = await DB.Users.GetAsync(data.UserId);

            if (dbUser.ProfileBackground == 0) // TODO: set default back
                return message;

            var background = await DB.ProfileBackgrounds.GetAsync(dbUser.ProfileBackground);

            if (background is null)
            {
                TemplateError($"No background found with ID {dbUser.ProfileBackground.ToString()}");
                return message;
            }

            return await ReplaceDataAsync(message, background.Url);
        }
    }
}
