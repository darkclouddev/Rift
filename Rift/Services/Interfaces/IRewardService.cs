using System.Threading.Tasks;

using Rift.Services.Reward;

namespace Rift.Services.Interfaces
{
    public interface IRewardService
    {
        Task DeliverToAsync(ulong userId, RewardBase reward);
        string Format(ItemReward reward);
        string ToPlainString(ItemReward reward);
        Task<string> FormatAsync(RoleReward reward);
        Task<string> FormatAsync(BackgroundReward reward);
    }
}
