using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface ICapsuleService
    {
        Task OpenAsync(ulong userId);
    }
}
