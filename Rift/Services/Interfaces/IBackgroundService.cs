using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface IBackgroundService
    {
        Task GetInventoryAsync(ulong userId);
        Task SetActiveAsync(ulong userId, int backgroundId);
    }
}
