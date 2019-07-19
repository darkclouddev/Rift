using System.Threading.Tasks;

namespace Rift.Services.Reward
{
    public class BackgroundReward : RewardBase
    {
        int backgroundId;

        public BackgroundReward()
        {
            Type = RewardType.Background;
        }

        public BackgroundReward SetId(int id)
        {
            backgroundId = id;
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            await DB.BackgroundInventory.AddAsync(userId, backgroundId).ConfigureAwait(false);
        }

        public override string ToString()
        {
            var dbBack = Task.Run(async () => await DB.ProfileBackgrounds.GetAsync(backgroundId)).Result;

            var text = "фон ";

            if (dbBack is null)
            {
                RiftBot.Log.Error($"{nameof(backgroundId)} does not exist!");
                text += "не найден";
                return text;
            }

            text += dbBack.Name;
            return text;
        }
    }
}
