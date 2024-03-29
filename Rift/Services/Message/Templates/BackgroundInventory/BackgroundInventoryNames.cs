using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.BackgroundInventory
{
    public class BackgroundInventoryNames : TemplateBase
    {
        public BackgroundInventoryNames() : base(nameof(BackgroundInventoryNames))
        {
        }
        
        const string NoItems = "-";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var invList = await DB.BackgroundInventory.GetAsync(data.UserId);

            if (invList is null || invList.Count == 0)
                return await ReplaceDataAsync(message, NoItems);

            var profileBackgrounds = await DB.ProfileBackgrounds.GetAllAsync();

            var replacement = string.Join('\n', invList.Select(x =>
            {
                var back = profileBackgrounds.FirstOrDefault(y => y.Id == x.BackgroundId);

                if (back is null)
                {
                    TemplateError($"Background ID {x.BackgroundId.ToString()} does not exist in DB!");
                    return "N/A";
                }

                return $"[{back.Name}]({back.Url})";
            }));

            return await ReplaceDataAsync(message, replacement);
        }
    }
}
