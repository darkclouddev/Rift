using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Inventory
{
    public class InventoryRolesAmount : TemplateBase
    {
        public InventoryRolesAmount() : base(nameof(InventoryRolesAmount))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var rolesCount = await DB.RoleInventory.CountAsync(data.UserId);
            return await ReplaceDataAsync(message, rolesCount.ToString());
        }
    }
}
