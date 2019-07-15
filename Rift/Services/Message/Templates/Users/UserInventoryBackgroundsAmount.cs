using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserInventoryBackgroundsAmount : TemplateBase
    {
        public UserInventoryBackgroundsAmount() : base(nameof(UserInventoryBackgroundsAmount))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var backgrounds = await DB.BackgroundInventory.CountAsync(data.UserId);
            return await ReplaceDataAsync(message, backgrounds.ToString());
        }
    }
}
