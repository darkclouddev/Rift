using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Inventory
{
    public class UserInventoryEssence : TemplateBase
    {
        public UserInventoryEssence() : base(nameof(UserInventoryEssence))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            return await ReplaceDataAsync(message, inventory.Essence.ToString());
        }
    }
}
