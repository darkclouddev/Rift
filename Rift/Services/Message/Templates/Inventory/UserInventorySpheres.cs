using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Inventory
{
    public class UserInventorySpheres : TemplateBase
    {
        public UserInventorySpheres() : base(nameof(UserInventorySpheres)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            return await ReplaceData(message, inventory.Spheres.ToString());
        }
    }
}
