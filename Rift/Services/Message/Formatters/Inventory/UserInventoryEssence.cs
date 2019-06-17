using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Inventory
{
    public class UserInventoryEssence : FormatterBase
    {
        public UserInventoryEssence() : base("$userInventoryEssence") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var inventory = await Database.GetUserInventoryAsync(data.UserId);
            return await ReplaceData(message, inventory.Essence.ToString());
        }
    }
}
