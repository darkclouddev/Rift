using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Gifts
{
    public class GiftNeededCoins : FormatterBase
    {
        public GiftNeededCoins() : base("$giftNeededCoins") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var inventory = await DB.Inventory.GetAsync(data.UserId);
            var coins = Settings.Economy.GiftPrice - inventory.Coins;

            return await ReplaceData(message, coins.ToString());
        }
    }
}
