using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserNextLevel : FormatterBase
    {
        public UserNextLevel() : base("$userNextLevel") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var level = await DB.Users.GetLevelAsync(data.UserId);

            return await ReplaceData(message, (level+1).ToString());
        }
    }
}
