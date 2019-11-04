using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface IEconomyService
    {
        void Init();
        List<ulong> SortedRating { get; set; }
        Task ShowActiveUsersAsync();
        Task ShowRichUsersAsync();
        Task ProcessMessageAsync(ulong userId);
        Task GetUserCooldownsAsync(ulong userId);
        Task GetUserProfileAsync(ulong userId);
        Task GetUserStatAsync(ulong userId);
        Task GetQuests(ulong userId);
        uint GetExpForLevel(uint level);
    }
}
