using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface IDailyService
    {
        Task CheckAsync(ulong userId);
    }
}
