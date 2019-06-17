using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserInventoryRolesAmount : FormatterBase
    {
        public UserInventoryRolesAmount() : base("$userInventoryRolesAmount") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var rolesCount = await Database.GetRoleInventoryCountAsync(data.UserId);
            return await ReplaceData(message, rolesCount.ToString());
        }
    }
}
