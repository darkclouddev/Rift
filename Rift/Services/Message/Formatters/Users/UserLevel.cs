using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserLevel : FormatterBase
    {
        public UserLevel() : base("$userLevel") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var level = await Database.GetUserLevelAsync(data.UserId);
            return await ReplaceData(message, level.ToString());
        }
    }
}
