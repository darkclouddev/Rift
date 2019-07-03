using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Inventory
{
    public class UserInventoryBotRespects : TemplateBase
    {
        public UserInventoryBotRespects() : base(nameof(UserInventoryBotRespects)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            return await ReplaceData(message, inventory.BonusBotRespect.ToString());
        }
    }
}
