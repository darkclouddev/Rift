using System.Linq;
using System.Threading.Tasks;

namespace Rift.Services.Reward
{
    public class BackgroundReward : RewardBase
    {
        public int BackgroundId { get; set; }

        public BackgroundReward()
        {
            Type = RewardType.Background;
        }

        public BackgroundReward SetBackgroundId(int id)
        {
            BackgroundId = id;
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            var inv = await DB.BackgroundInventory.GetAsync(userId);

            if (!(inv is null) && inv.Any(x => x.UserId == userId && x.BackgroundId == BackgroundId))
                return;

            await DB.BackgroundInventory.AddAsync(userId, BackgroundId).ConfigureAwait(false);
        }

        public override string ToString()
        {
            var dbBack = Task.Run(async () => await DB.ProfileBackgrounds.GetAsync(BackgroundId)).Result;

            var text = "фон ";

            if (dbBack is null)
            {
                RiftBot.Log.Error($"{nameof(BackgroundId)} does not exist!");
                text += "не найден";
                return text;
            }

            text += dbBack.Name;
            return text;
        }
    }
}
