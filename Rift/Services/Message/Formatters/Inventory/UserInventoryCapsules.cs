using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Inventory
{
    public class UserInventoryCapsules : FormatterBase
    {
        public UserInventoryCapsules() : base("$userInventoryCapsules") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var inventory = await Database.GetUserInventoryAsync(data.UserId);
            return await ReplaceData(message, inventory.Capsules.ToString());
        }
    }
}
