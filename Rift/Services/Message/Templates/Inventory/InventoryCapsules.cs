using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Inventory
{
    public class InventoryCapsules : TemplateBase
    {
        public InventoryCapsules() : base(nameof(InventoryCapsules))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            return await ReplaceDataAsync(message, inventory.Capsules.ToString());
        }
    }
}
