using System.Threading.Tasks;

namespace Rift.Services.Reward
{
    public abstract class RewardBase
    {
        public RewardType Type;
        public abstract Task DeliverToAsync(ulong userId);
    }

    public enum RewardType
    {
        Undefined = 0,
        Item = 1,
        Role = 2,
    }
}
