using Newtonsoft.Json;

namespace Rift.Services.Reward
{
    public abstract class RewardBase
    {
        [JsonIgnore]
        public RewardType Type { get; protected set; }
    }

    public enum RewardType
    {
        Item = 1,
        Role = 2,
        Background = 3,
        Capsule = 4,
        Chest = 5,
        Gift = 6,
        Sphere = 7,
    }
}
