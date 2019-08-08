using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserNextLevel : TemplateBase
    {
        public UserNextLevel() : base(nameof(UserNextLevel))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var level = await DB.Users.GetLevelAsync(data.UserId);

            return await ReplaceDataAsync(message, (level + 1).ToString());
        }
    }
}
