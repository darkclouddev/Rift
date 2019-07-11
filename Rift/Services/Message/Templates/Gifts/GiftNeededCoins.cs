using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftNeededCoins : TemplateBase
    {
        public GiftNeededCoins() : base(nameof(GiftNeededCoins))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            var coins = Settings.Economy.GiftPrice - inventory.Coins;

            return await ReplaceDataAsync(message, coins.ToString());
        }
    }
}
