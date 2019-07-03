using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserInventoryRolesAmount : TemplateBase
    {
        public UserInventoryRolesAmount() : base(nameof(UserInventoryRolesAmount)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var rolesCount = await DB.RoleInventory.GetCountAsync(data.UserId);
            return await ReplaceData(message, rolesCount.ToString());
        }
    }
}
