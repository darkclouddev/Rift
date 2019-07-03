using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserLevel : TemplateBase
    {
        public UserLevel() : base(nameof(UserLevel)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var level = await DB.Users.GetLevelAsync(data.UserId);
            return await ReplaceData(message, level.ToString());
        }
    }
}
