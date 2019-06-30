using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserProfileBackgroundUrl : FormatterBase
    {
        public UserProfileBackgroundUrl() : base("$userProfileBackgroundUrl") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var dbUser = await DB.Users.GetAsync(data.UserId);
            
            if (dbUser.ProfileBackground == 0)
                return message;

            var background = await DB.ProfileBackgrounds.GetAsync(dbUser.ProfileBackground);

            if (background is null)
                return message;

            return await ReplaceData(message, background.Url);
        }
    }
}