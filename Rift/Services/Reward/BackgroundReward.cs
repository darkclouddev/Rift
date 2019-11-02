namespace Rift.Services.Reward
{
    public class BackgroundReward : RewardBase
    {
        public int BackgroundId { get; private set; }

        public BackgroundReward()
        {
            Type = RewardType.Background;
        }

        public BackgroundReward SetId(int id)
        {
            BackgroundId = id;
            return this;
        }
    }
}
